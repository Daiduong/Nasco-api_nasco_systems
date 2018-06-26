using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Helper;
using System.Dynamic;
using System.Linq.Expressions;
using static NascoWebAPI.Helper.Common.Constants;
using NascoWebAPI.Models;

namespace NascoWebAPI.Data
{
    public class LadingRepository : Repository<Lading>, ILadingRepository
    {
        private readonly ILadingHistoryRepository _iLadingHistoryRepository;
        private readonly IDepartmentRepository _iDepartmentRepository;
        private readonly IOfficerRepository _iOfficerRepository;
        private readonly IPostOfficeRepository _iPostOfficeRepository;
        private readonly ITypeReasonRepository _iTypeReasonRepository;

        private const int _STATUS_PickUp = (int)StatusLading.ChoLayHang;

        private static int[] _STATUS_LATLNG_USER = { (int)StatusLading.DaLayHang, (int)StatusLading.DangTrungChuyen,
                                                        (int)StatusLading.DangPhat};
        private static int[] _STATUS_LATLNG_POCURRENT = { (int)StatusLading.KhaiThacThongTin, (int)StatusLading.ChuaNhapKho,
                                                        (int)StatusLading.NhapKho, (int)StatusLading.XuatKho, (int)StatusLading.XuatKhoTrungChuyen ,(int)StatusLading.PhatKhongTC };
        private static int[] _STATUS_LATLNG_TO = { (int)StatusLading.DaChuyenHoan, (int)StatusLading.ThanhCong };

        public LadingRepository(ApplicationDbContext context) : base(context)
        {
            _iOfficerRepository = new OfficerRepository(context);
            _iDepartmentRepository = new DepartmentRepository(context);
            _iTypeReasonRepository = new TypeReasonRepository(context);
            _iPostOfficeRepository = new PostOfficeRepository(context);
            _iLadingHistoryRepository = new LadingHistoryRepository(context);
        }

        public LadingRepository(ApplicationDbContext context,
            IOfficerRepository iOfficerRepository,
            ILadingHistoryRepository iLadingHistoryRepository,
            IDepartmentRepository iDepartmentRepository,
            ITypeReasonRepository iTypeReasonRepository,
            IPostOfficeRepository iPostOfficerRepository
            ) : base(context)
        {
            _iLadingHistoryRepository = iLadingHistoryRepository;
            _iOfficerRepository = iOfficerRepository;
            _iDepartmentRepository = iDepartmentRepository;
            _iPostOfficeRepository = iPostOfficerRepository;
            _iTypeReasonRepository = new TypeReasonRepository(context);
        }


        public virtual void InsertAndInsertLadingHistory(Lading entity)
        {
            try
            {
                entity.ModifiedDate = DateTime.Now;
                entity.CreateDate = DateTime.Now;
                if ((entity.Weight ?? 0) <= 0) entity.Weight = 0.1;
                this.Insert(entity);
                this.SaveChanges();

                LadingHistory ldHistory = new LadingHistory();
                ldHistory.LadingId = entity.Id;
                ldHistory.OfficerId = entity.ModifiedBy;

                //var office = _iOfficerRepository.GetSingle(o => o.OfficerID == entity.OfficerId);
                //var department = _iDepartmentRepository.GetSingle(o => o.DeparmentID == office.DeparmentID);

                ldHistory.PostOfficeId = entity.POCurrent;
                ldHistory.Status = entity.Status;
                ldHistory.DateTime = DateTime.Now;
                ldHistory.Note = entity.Noted;
                ldHistory = UpdateLatLngHistory(ldHistory);
                _iLadingHistoryRepository.Insert(ldHistory);
                _iLadingHistoryRepository.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error", ex);
            }
        }
        public override void Update(Lading entity)
        {
            entity.ModifiedDate = DateTime.Now;
            base.Update(entity);
        }
        public virtual void UpdateAndInsertLadingHistory(Lading entity, Officer currentUser, int? typeReasonID = null, string location = null, string note = "")
        {
            try
            {
                entity.ModifiedDate = DateTime.Now;
                if (typeReasonID.HasValue)
                {
                    var typeReson = _iTypeReasonRepository.GetSingle(obj => typeReasonID.Value == obj.TypeReasonID);
                    if (typeReson != null)
                    {
                        if (typeReson.IsText)
                        {
                            note = typeReson.TypeReasonName + ": " + note;
                        }
                        else
                        {
                            note = typeReson.TypeReasonName;
                        }
                    }
                }
                if (entity.Status.Value == (int)StatusLading.ThanhCong && (entity.COD ?? 0) > 0)
                {
                    entity.StatusCOD = (int)StatusCOD.DaThu;
                    entity.CODKeepBy = currentUser.OfficerID;
                    entity.PostOfficeKeepCOD = currentUser.PostOfficeId;
                }
                if (entity.Status.Value == (int)StatusLading.ThanhCong && entity.PaymentType == (int)PaymentType.Recipient)
                {
                    entity.PaymentAmount = true;
                    entity.AmountKeepBy = currentUser.OfficerID;
                    entity.PostOfficeKeepAmount = currentUser.PostOfficeId;
                }
                if (entity.Status.Value == (int)StatusLading.HoanGoc)
                {
                    entity.Return = true;
                }
                if ((entity.Status.Value == (int)StatusLading.ThanhCong
                    || entity.Status.Value == (int)StatusLading.PhatKhongTC) && !(entity.Return ?? false))
                {
                    entity.TimesDelivery = (entity.TimesDelivery ?? 0) + 1;
                }
                else if ((entity.Status.Value == (int)StatusLading.DaChuyenHoan
                || entity.Status.Value == (int)StatusLading.PhatKhongTC) && (entity.Return ?? false))
                {
                    entity.TimesDelivery = (entity.TimesDeliveryReturn ?? 0) + 1;
                }
                var statusDelivery = new int[] { (int)StatusLading.DaChuyenHoan, (int)StatusLading.ThanhCong };
                if (entity.BKDeliveryId.HasValue && entity.Status.HasValue && statusDelivery.Contains(entity.Status.Value))
                {
                    var bkDelivery = _context.BKDeliveries.SingleOrDefault(o => o.ID_BK_Delivery == entity.BKDeliveryId.Value);
                    if (bkDelivery != null)
                    {
                        bkDelivery.TotalLadingDeliveried = (bkDelivery.TotalLadingDeliveried ?? 0) + 1;
                        if (bkDelivery.TotalLadingDeliveried == bkDelivery.TotalLading)
                        {
                            bkDelivery.Status = 702;
                        }
                        else if (bkDelivery.Status == 700)
                        {
                            bkDelivery.Status = 701;
                        }
                    }
                }
                this.Update(entity);
                LadingHistory ldHistory = new LadingHistory
                {
                    LadingId = entity.Id,
                    OfficerId = currentUser.OfficerID,
                    Location = location,
                    //var office = _iOfficerRepository.GetSingle(o => o.OfficerID == entity.OfficerId);
                    //var postOffice = _iDepartmentRepository.GetSingle(o => o.DeparmentID == office.DeparmentID);
                    PostOfficeId = currentUser.PostOfficeId,
                    Status = entity.Status,
                    DateTime = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    TypeReason = typeReasonID,
                    Note = note
                };
                ldHistory = UpdateLatLngHistory(ldHistory);
                _iLadingHistoryRepository.Insert(ldHistory);
                _iLadingHistoryRepository.SaveChanges();

                this.SaveChanges();
                if (entity.Status == (int)StatusLading.ThanhCong && _context.Services.Any(o => o.ServiceID == entity.ServiceId && o.GSId.Value == 1))
                {
                    var task = ApiCustomer.LadingSendSMS((int)entity.Id, entity.POTo ?? 0, 4);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error", ex);
            }
        }

        public async Task<IEnumerable<Lading>> GetPickUpAsync(Officer officer)
        {
            var department = await _iDepartmentRepository.GetSingleAsync(d => d.DeparmentID == officer.DeparmentID);
            if (department != null && department.PostOfficeID.HasValue)
            {
                var departments = await _iDepartmentRepository.GetAsync(d => d.PostOfficeID == department.PostOfficeID);
                var officers = await _iOfficerRepository.GetAsync(o => departments.Any(r => r.DeparmentID == o.DeparmentID));
                var ladings = await this.GetAsync(obj => obj.POCurrent == department.PostOfficeID && _STATUS_PickUp == obj.Status.Value);
                var result = new Dictionary<Lading, double>();
                if (officers.Count() > 0)
                {
                    foreach (var lading in ladings)
                    {
                        if (lading.LatFrom.HasValue && lading.LngFrom.HasValue)
                        {
                            int index = 0;
                            dynamic minDistance = new ExpandoObject();
                            minDistance.Distance = 0;
                            minDistance.Id = 0;
                            foreach (var officer1 in officers)
                            {
                                if (officer1.LatDynamic.IsNumeric() && officer1.LngDynamic.IsNumeric())
                                {
                                    var distance = GeoHelper.distance(lading.LatFrom.Value, lading.LngFrom.Value, Convert.ToDouble(officer1.LatDynamic), Convert.ToDouble(officer.LngDynamic), 'K');
                                    if (index == 0 || minDistance.Distance > distance)
                                    {
                                        minDistance.Distance = distance;
                                        minDistance.Id = officer1.OfficerID;
                                        index++;
                                    }
                                }
                            }
                            if (minDistance.Id == officer.OfficerID)
                            {
                                result.Add(lading, minDistance.Distance);
                            }
                        }
                    }
                    return result.OrderBy(obj => obj.Value).Select(obj => obj.Key);
                }
            }
            return null;
        }

        public async Task<LadingHistory> GetLastLadingHistoryAsync(long ladingId)
        {
            return await _iLadingHistoryRepository.GetFirstAsync(l => l.LadingId == ladingId, l => l.OrderByDescending(r => r.Id));
        }
        public LadingHistory GetLastLadingHistory(long ladingId)
        {
            return _iLadingHistoryRepository.GetFirst(l => l.LadingId == ladingId, l => l.OrderByDescending(r => r.Id));
        }
        public async Task<IEnumerable<LadingHistory>> GetLastLadingHistoryByUser(int officerID)
        {
            return await _iLadingHistoryRepository.GetAsync(l => l.OfficerId == officerID && this.GetLastLadingHistory(l.LadingId.Value).Id == l.Id, l => l.OrderByDescending(r => r.Id));
        }
        private LadingHistory UpdateLatLngHistory(LadingHistory ldHistory)
        {
            var office = new Officer();
            var postOffice = _iPostOfficeRepository.GetFirst(o => o.PostOfficeID == ldHistory.PostOfficeId);
            var lading = this.GetFirst(l => l.Id == ldHistory.LadingId);

            if (ldHistory.Status.HasValue)
            {
                if (_STATUS_LATLNG_USER.Contains(ldHistory.Status.Value))
                {
                    if (ldHistory.Status.Value == _STATUS_LATLNG_USER[0])
                    {
                        office = _iOfficerRepository.GetFirst(o => o.OfficerID == lading.OfficerPickup);
                    }
                    else
                    {
                        office = _iOfficerRepository.GetFirst(o => o.OfficerID == lading.OfficerDelivery);
                    }
                    if (office != null)
                    {
                        if (office.LatDynamic.IsNumeric())
                        {
                            ldHistory.Lat = Convert.ToDouble(office.LatDynamic);
                        }
                        if (office.LngDynamic.IsNumeric())
                        {
                            ldHistory.Lng = Convert.ToDouble(office.LngDynamic);
                        }
                    }
                }
                else if (_STATUS_LATLNG_POCURRENT.Contains(ldHistory.Status.Value) && postOffice != null)
                {
                    ldHistory.Lat = postOffice.Lat;
                    ldHistory.Lng = postOffice.Lng;
                }
                else if (lading != null)
                {
                    if (_STATUS_LATLNG_TO.Contains(ldHistory.Status.Value))
                    {
                        ldHistory.Lat = lading.LatTo;
                        ldHistory.Lng = lading.LngTo;
                    }
                    else
                    {
                        ldHistory.Lat = lading.LatFrom;
                        ldHistory.Lng = lading.LngFrom;
                    }
                }
            }
            return ldHistory;
        }
        public async Task<IEnumerable<Lading>> GetLadingReport(int officerID, int reportType)
        {
            ReportType eReportType = (ReportType)Enum.ToObject(typeof(ReportType), reportType);
            IEnumerable<Lading> result = Enumerable.Empty<Lading>();
            switch (eReportType)
            {
                case ReportType.PhaiDiLay:
                    result = await this.GetAsync(l => l.OfficerPickup == officerID && (l.Status == (int)StatusLading.ChoLayHang || l.Status == (int)StatusLading.DangLayHang));
                    break;
                case ReportType.DaDiLay:
                    result = await this.GetAsync(l => l.OfficerPickup == officerID && (l.Status == (int)StatusLading.DaLayHang));
                    break;
                case ReportType.DaPhanDiGiao:
                    result = await this.GetAsync(l => l.OfficerDelivery == officerID && (l.Status == (int)StatusLading.XuatKho));
                    break;
                case ReportType.DangGiu:
                    result = await this.GetAsync(l => l.OfficerDelivery == officerID && (l.Status == (int)StatusLading.DangPhat));
                    break;
                case ReportType.PhatTCNgay:
                    result = await this.GetAsync(l => l.OfficerDelivery == officerID && (int)StatusLading.ThanhCong == l.Status && l.FinishDate.Value.Date == DateTime.Now.Date);
                    break;
                case ReportType.PhatTCThang:
                    result = await this.GetAsync(l => l.OfficerDelivery == officerID && (int)StatusLading.ThanhCong == l.Status && l.FinishDate.Value.Month == DateTime.Now.Month && l.FinishDate.Value.Year == DateTime.Now.Year);
                    break;
                case ReportType.ChuyenHoanNgay:
                    result = await this.GetAsync(l => l.OfficerDelivery == officerID && (int)StatusLading.DaChuyenHoan == l.Status && l.FinishDate.Value.Date == DateTime.Now.Date);
                    break;
                case ReportType.ChuyenHoanThang:
                    result = await this.GetAsync(l => l.OfficerDelivery == officerID && (int)StatusLading.DaChuyenHoan == l.Status && l.FinishDate.Value.Month == DateTime.Now.Month && l.FinishDate.Value.Year == DateTime.Now.Year);
                    break;
                case ReportType.CODDangGiuNgay:
                    result = await this.GetAsync(l => l.CODKeepBy == officerID && (int)StatusCOD.DaThu == l.StatusCOD && (int)StatusLading.ThanhCong == l.Status && l.COD.Value > 0 && l.FinishDate.Value.Date == DateTime.Now.Date);
                    break;
                case ReportType.CODDangGiuThang:
                    result = await this.GetAsync(l => l.CODKeepBy == officerID && (int)StatusCOD.DaThu == l.StatusCOD && (int)StatusLading.ThanhCong == l.Status && l.COD.Value > 0 && l.FinishDate.Value.Month == DateTime.Now.Month && l.FinishDate.Value.Year == DateTime.Now.Year);
                    break;

            }
            return result;
        }
        public async Task<double> GetSumLadingReport(int officerID, int reportType)
        {
            var result = await GetLadingReport(officerID, reportType);
            if (reportType == (int)ReportType.CODDangGiuNgay || reportType == (int)ReportType.CODDangGiuThang)
            {
                return result.Sum(obj => obj.COD.Value);
            }
            else
            {
                return result.Count();
            }
        }
        public string CodeGenerationByLocationCode(int locationId, int id)
        {
            var locationCode = string.Empty;
            var code = string.Empty;
            var location = _context.Locations.FirstOrDefault(p => p.LocationId == locationId);
            if (location != null)
            {
                locationCode = location.Code_Local;
                code = locationCode + id.ToString("D7");
                //var lastLading = _context.Ladings.Where(p => p.State == (int)StatusSystem.Enable && p.Code.Contains(locationCode)).Max(p => p.Code);
                //if (lastLading != null)
                //{
                //    var newCode = lastLading.Replace(locationCode, "");
                //    var numberCode = int.Parse(newCode) + 1;
                //    var codeTemp = numberCode.ToString("D7");
                //    code = locationCode + codeTemp;
                //}
                //else
                //    code = locationCode + "0000001";
            }

            return code;
        }
        public bool EqualsCode(string code)
        {
            return (this.Any(p => p.Code.ToUpper() == code.ToUpper() && p.Status != (int)StatusLading.BillTrang));
        }

    }
}