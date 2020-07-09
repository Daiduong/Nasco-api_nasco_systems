using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Helper;
using System.Dynamic;
using System.Linq.Expressions;
using NascoWebAPI.Services;
using static NascoWebAPI.Helper.Common.Constants;
using NascoWebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Data.SqlClient;
using NascoWebAPI.Models.Response;

namespace NascoWebAPI.Data
{
    public class LadingRepository : Repository<Lading>, ILadingRepository
    {
        private const int _STATUS_PickUp = (int)StatusLading.ChoLayHang;

        private static int[] _STATUS_LATLNG_USER = { (int)StatusLading.DaLayHang, (int)StatusLading.DangTrungChuyen,
                                                        (int)StatusLading.DangPhat};
        private static int[] _STATUS_LATLNG_POCURRENT = { (int)StatusLading.KhaiThacThongTin, (int)StatusLading.ChuaNhapKho,
                                                        (int)StatusLading.NhapKho, (int)StatusLading.XuatKho, (int)StatusLading.XuatKhoTrungChuyen ,(int)StatusLading.PhatKhongTC };
        private static int[] _STATUS_LATLNG_TO = { (int)StatusLading.DaChuyenHoan, (int)StatusLading.ThanhCong };

        public LadingRepository(ApplicationDbContext context) : base(context)
        {
        }
        public virtual void InsertAndInsertLadingHistory(Lading entity)
        {
            try
            {
                var iLadingHistoryRepository = new LadingHistoryRepository(_context);
                entity.ModifiedDate = DateTime.Now;
                entity.CreateDate = DateTime.Now;
                if ((entity.Weight ?? 0) <= 0) entity.Weight = 0.1;
                this.Insert(entity);
                this.SaveChanges();

                LadingHistory ldHistory = new LadingHistory();
                ldHistory.LadingId = entity.Id;
                ldHistory.OfficerId = entity.ModifiedBy;

                //var office = iOfficerRepository.GetSingle(o => o.OfficerID == entity.OfficerId);
                //var department = iDepartmentRepository.GetSingle(o => o.DeparmentID == office.DeparmentID);

                ldHistory.PostOfficeId = entity.POCurrent;
                ldHistory.Status = entity.Status;
                ldHistory.DateTime = DateTime.Now;
                ldHistory.Note = entity.Noted;
                ldHistory = UpdateLatLngHistory(ldHistory);
                iLadingHistoryRepository.Insert(ldHistory);
                iLadingHistoryRepository.SaveChanges();
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
                var iLadingHistoryRepository = new LadingHistoryRepository(_context);
                entity.ModifiedDate = DateTime.Now;
                if (typeReasonID.HasValue)
                {
                    var iTypeReasonRepository = new TypeReasonRepository(_context);
                    var typeReson = iTypeReasonRepository.GetSingle(obj => typeReasonID.Value == obj.TypeReasonID);
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
                if (entity.Status.Value == (int)StatusLading.ThanhCong && !(entity.PaymentAmount ?? false) && entity.PaymentType == (int)PaymentType.Recipient)
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
                    //var office = iOfficerRepository.GetSingle(o => o.OfficerID == entity.OfficerId);
                    //var postOffice = iDepartmentRepository.GetSingle(o => o.DeparmentID == office.DeparmentID);
                    PostOfficeId = currentUser.PostOfficeId,
                    Status = entity.Status,
                    DateTime = DateTime.Now,
                    CreatedDate = DateTime.Now,
                    TypeReason = typeReasonID,
                    Note = note
                };
                ldHistory = UpdateLatLngHistory(ldHistory);
                iLadingHistoryRepository.Insert(ldHistory);
                iLadingHistoryRepository.SaveChanges();

                this.SaveChanges();
                if (entity.Status == (int)StatusLading.ThanhCong && _context.Services.Any(o => o.ServiceID == entity.ServiceId && (o.IsSendSMS ?? false)))
                {
                    var task = ApiCustomer.LadingSendSMS((int)entity.Id, entity.POTo ?? 0, 4);
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error", ex);
            }
        }
        public async Task<LadingHistory> GetLastLadingHistoryAsync(long ladingId)
        {
            var iLadingHistoryRepository = new LadingHistoryRepository(_context);
            return await iLadingHistoryRepository.GetFirstAsync(l => l.LadingId == ladingId, l => l.OrderByDescending(r => r.Id));
        }
        public LadingHistory GetLastLadingHistory(long ladingId)
        {
            var iLadingHistoryRepository = new LadingHistoryRepository(_context);
            return iLadingHistoryRepository.GetFirst(l => l.LadingId == ladingId, l => l.OrderByDescending(r => r.Id));
        }
        public async Task<IEnumerable<LadingHistory>> GetLastLadingHistoryByUser(int officerID)
        {
            var iLadingHistoryRepository = new LadingHistoryRepository(_context);
            return await iLadingHistoryRepository.GetAsync(l => l.OfficerId == officerID && this.GetLastLadingHistory(l.LadingId.Value).Id == l.Id, l => l.OrderByDescending(r => r.Id));
        }
        private LadingHistory UpdateLatLngHistory(LadingHistory ldHistory)
        {
            var iLadingHistoryRepository = new LadingHistoryRepository(_context);
            var iOfficerRepository = new OfficerRepository(_context);
            var iPostOfficeRepository = new PostOfficeRepository(_context);
            var office = new Officer();
            var postOffice = iPostOfficeRepository.GetFirst(o => o.PostOfficeID == ldHistory.PostOfficeId);
            var lading = this.GetFirst(l => l.Id == ldHistory.LadingId);

            if (ldHistory.Status.HasValue)
            {
                if (_STATUS_LATLNG_USER.Contains(ldHistory.Status.Value))
                {
                    if (ldHistory.Status.Value == _STATUS_LATLNG_USER[0])
                    {
                        office = iOfficerRepository.GetFirst(o => o.OfficerID == lading.OfficerPickup);
                    }
                    else
                    {
                        office = iOfficerRepository.GetFirst(o => o.OfficerID == lading.OfficerDelivery);
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
            }

            return code;
        }
        public bool EqualsCode(string code)
        {
            return (this.Any(p => p.Code.ToUpper() == code.ToUpper() && p.Status != (int)StatusLading.BillTrang));
        }
        public async Task<ResultModel<Lading>> InsertEMS(int currentUserId, int customerContactId, int serviceId, string code, int statusId = (int)StatusLading.DaLayHang)
        {
            var result = new ResultModel<Lading>();
            if (string.IsNullOrEmpty(code))
                return result.Init(message: "Mã vận đơn không được bỏ trống");
            code = code.ToUpper();
            if (this.Any(x => x.Code != null && x.Code.ToUpper().Equals(code) && x.State == 0))
                return result.Init(message: "Vận đơn " + code + " đã tồn tại trên hệ thống");
            if (!_context.Services.Any(x => x.ServiceID == serviceId))
                return result.Init(message: "Không tìm thấy dịch vụ trên hệ thống");
            var customerContact = await _context.CustomerContacts.SingleOrDefaultAsync(x => x.Id == customerContactId);
            if (customerContact == null || !customerContact.CustomerId.HasValue)
            {
                return result.Init(message: "Không tìm thấy khách hàng này trên hệ thống");
            }
            var customerId = customerContact.CustomerId.Value;
            var customer = await _context.Customers.SingleOrDefaultAsync(x => x.CustomerID == customerId && x.State == 0);
            if (customer == null)
            {
                return result.Init(message: "Không tìm thấy khách hàng này trên hệ thống");
            }
            try
            {
                var officer = _context.Set<Officer>().Include(o => o.PostOffice).Single(o => o.OfficerID == currentUserId);
                int currentPOId = officer.PostOfficeId.Value;
                var priceListRepository = new PriceListRepository(_context);
                var postOfficeRepository = new PostOfficeRepository(_context);

                //GetPriceList by customer
                var cityRecipientId = customerContact.CityId == (int)LocationEnum.HAN ? (int)LocationEnum.SGN : (int)LocationEnum.HAN;
                var poToId = _context.Locations.FirstOrDefault(x => x.LocationId == cityRecipientId)?.PostOfficeId;
                var centerFromId = (await postOfficeRepository.GetBranch(currentPOId) ?? new PostOffice()).PostOfficeID;
                var centerToId = (await postOfficeRepository.GetBranch(poToId ?? 0) ?? new PostOffice()).PostOfficeID;

                var lading = new Lading
                {
                    Code = code,
                    ShopCode = customerContact.Code,
                    OfficerPickup = currentUserId,
                    OfficerId = currentUserId,
                    POFrom = currentPOId,
                    POCurrent = currentPOId,
                    POCreated = currentPOId,
                    CreateDate = DateTime.Now,
                    ModifiedDate = DateTime.Now,
                    ModifiedBy = currentUserId,
                    SenderId = customerId,
                    ServiceId = serviceId,
                    PaymentType = (int)PaymentType.Month,
                    NameFrom = customerContact.ContactName,
                    PhoneFrom = customerContact.ContactPhone,
                    AddressFrom = customerContact.ContactAddress,
                    LatFrom = customerContact.Lat,
                    LngFrom = customerContact.Lng,
                    CitySendId = customerContact.CityId,
                    DistrictFrom = customerContact.DistrictId,
                    Status = statusId,
                    PriceListId = priceListRepository.GetListPriceListByCustomer(customerId, true).FirstOrDefault()?.PriceListID,
                    State = 0,
                    OrderByService = 1,
                    Number = 1,
                    CityRecipientId = cityRecipientId,
                    POTo = poToId,
                    CenterFrom = centerFromId,
                    CenterTo = centerToId,
                    RDFrom = (int)DeliveryReceiveType.DC_DC
                };
                this.Insert(lading);
                this.SaveChanges();
                _context.LadingHistories.Add(EntityHelper.GetLadingHistory(currentUserId, currentPOId, lading.Id, lading.Status ?? 0, officer.LatDynamic, officer.LngDynamic, statusId != (int)StatusLading.DaLayHang ? officer.PostOffice?.POName : ""));
                this.SaveChanges();
                result.Error = 0;
                result.Data = lading;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<ResultModel<dynamic>> UpdateEMS()
        {
            var result = new ResultModel<dynamic>();
            try
            {
                var ladingIds = _context.Ladings
                    .Join(_context.Customers.Where(x => x.PartnerId == (int)EMSHelper.PARTNER_ID), x => x.SenderId, y => y.CustomerID, (x, y) => x)
                    .Where(x => x.State == 0 && !x.RecipientId.HasValue)
                    .Select(x => x.Id).ToList();
                await Task.WhenAll(ladingIds.Select(async (id) =>
                 {
                     using (var context = new ApplicationDbContext())
                     {
                         var EMSService = new EMSService();
                         var googleMapService = new GoogleMapService();

                         var locationRespository = new LocationRepository(context);
                         var ladingRespository = new LadingRepository(context);
                         var recipientRepository = new RecipientRepository(context);
                         var priceRepository = new PriceRepository(context);
                         var ladingMapServiceRepository = new LadingMapServiceRepository(context);
                         var timeLineRepository = new TimeLineRepository(context);

                         var lading = await ladingRespository.GetFirstAsync(x => x.Id == id);
                         var ladingEMSResponse = await EMSService.GetLading(lading.Code);
                         if (ladingEMSResponse.Error == 0)
                         {
                             var ladingEMS = ladingEMSResponse.Data;

                             var distanceFromResponse = await googleMapService.GetDistancePostOffice(lading.POFrom ?? 0, lading.LatFrom ?? 0, lading.LngFrom ?? 0);
                             lading.DistanceFrom = distanceFromResponse.Value;
                             int provinceId = 0;
                             if (!string.IsNullOrEmpty(ladingEMS.To_Province))
                             {
                                 int.TryParse(ladingEMS.To_Province.Substring(0, 2), out provinceId);
                                 provinceId *= 10000;
                             }
                             lading.CityRecipientId = locationRespository.GetFirst(x => x.LocationEMSId.HasValue && x.LocationEMSId == provinceId)?.LocationId;
                             lading.Noted = ladingEMS.Note;
                             lading.NameTo = ladingEMS.To_Name;
                             lading.PhoneTo = ladingEMS.To_Phone;
                             if (!string.IsNullOrEmpty(ladingEMS.To_Address))
                             {
                                 if (!ladingEMS.To_Address.Contains(ladingEMS.To_Province))
                                     ladingEMS.To_Address = ladingEMS.To_Address + " " + ladingEMS.To_Province;
                                 lading.AddressTo = ladingEMS.To_Address;
                                 lading.AddressNoteTo = ladingEMS.To_Address;
                                 var googleMapResponse = await googleMapService.GetLocation(new GeocoderRequest()
                                 {
                                     Address = lading.AddressTo
                                 });
                                 if (googleMapResponse.Error == 0)
                                 {
                                     lading.DistrictTo = googleMapResponse.Data.DistrictId;
                                     lading.LatTo = googleMapResponse.Data.Lat;
                                     lading.LngTo = googleMapResponse.Data.Lng;
                                     if (!lading.CityRecipientId.HasValue)
                                     {
                                         lading.CityRecipientId = googleMapResponse.Data.CityId;
                                     }
                                     var recipient = await recipientRepository.GetFirstAsync(x => x.Phone == lading.PhoneTo) ?? new Recipient();
                                     recipient.Name = lading.NameTo;
                                     recipient.Lng = lading.LngTo;
                                     recipient.Lat = lading.LatTo;
                                     recipient.Phone = lading.PhoneTo;
                                     recipient.Address = lading.AddressTo;
                                     recipient.AddressNote = lading.AddressNoteTo;
                                     recipient.CityId = lading.CityRecipientId;
                                     recipient.DistrictId = lading.DistrictTo;
                                     recipient.State = 0;
                                     if (recipient.Id == 0)
                                     {
                                         recipientRepository.Insert(recipient);
                                         await context.SaveChangesAsync();
                                     }
                                     lading.RecipientId = recipient.Id;
                                     if (lading.CityRecipientId.HasValue)
                                     {
                                         var distanceToResponse = await googleMapService.GetPostOfficeMinDistance((int)PostOfficeMethod.TO, lading.CityRecipientId ?? 0, lading.LatTo ?? 0, lading.LngTo ?? 0, (lading.Weight ?? 0) > 30);
                                         lading.POTo = distanceToResponse.Key;
                                         lading.DistanceTo = distanceToResponse.Value;
                                     }
                                 }
                             }
                             lading.COD = ladingEMS.Amount;
                             lading.Weight = Math.Round(ladingEMS.Weight / 1000, 2);
                             lading.StructureID = EMSHelper.ConvertToStructure(ladingEMS.Class);
                             if (lading.CitySendId.HasValue && lading.CityRecipientId.HasValue && lading.RDFrom.HasValue
                               && lading.ServiceId.HasValue && lading.POTo.HasValue && lading.POCurrent.HasValue)
                             {
                                 var level = context.PostOffices.FirstOrDefault(x => x.PostOfficeID == lading.POTo)?.Level ?? 0;
                                 var expectedTimes = await timeLineRepository.GetListExpectedTime(lading.CitySendId ?? 0, lading.CityRecipientId ?? 0, lading.ServiceId ?? 0, lading.POCurrent ?? 0, level, lading.RDFrom ?? 0);
                                 if (expectedTimes.Count() > 0)
                                 {
                                     var expectedTime = expectedTimes.First();
                                     lading.ExpectedTimeTransfer = expectedTime.DateTimeStart;
                                     lading.ExpectedTimeDelivery = expectedTime.DateTimeEnd;
                                     lading.ExpectedTimeTakeOff = expectedTime.DateTimeTakeOff;
                                 }
                             }
                             var ladingModel = new LadingViewModel();
                             ladingModel.CopyFromOject(lading);
                             ladingModel.UpdateServiceOthers();
                             var computed = await priceRepository.Computed(ladingModel);
                             if (computed.Error == 0)
                             {
                                 var computedPrice = computed.Data;
                                 lading.Amount = computedPrice.TotalCharge;
                                 lading.DiscountAmount = computedPrice.DiscountAmount;
                                 lading.PriceVAT = computedPrice.ChargeVAT;
                                 lading.TotalPriceDVGT = computedPrice.ChargeAddition;
                                 lading.PriceMain = computedPrice.ChargeMain;
                                 lading.PPXDPercent = computedPrice.ChargeFuel;
                                 lading.PriceOther = computedPrice.Surcharge;
                                 if (computedPrice.ServiceOthers != null && computedPrice.ServiceOthers.Count() > 0)
                                 {
                                     await ladingMapServiceRepository.AddOrEdit((int)lading.Id, computedPrice.ServiceOthers);
                                 }
                             }

                             ladingRespository.Update(lading);
                             await context.SaveChangesAsync();
                         }
                     }
                 }));
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<ResultModel<dynamic>> UpdateToPartner(int partnerId, int ladingId, int poCurrentId, int statusId, DateTime? datetime)
        {
            var result = new ResultModel<dynamic>();
            var lading = await this.GetFirstAsync(x => x.Id == ladingId && x.Sender != null && x.Sender.PartnerId == ladingId, null, x => x.Sender);
            if (lading == null)
                return result.Init(message: "Không tìm thấy thông tin vận đơn");
            var poCurrent = await _context.PostOffices.FirstOrDefaultAsync(x => x.PostOfficeID == poCurrentId);
            if (poCurrent == null)
                return result.Init(message: "Không tìm thấy thông tin bưu cục");
            var status = await _context.LadingStatuses.FirstOrDefaultAsync(x => x.StatusId == statusId);
            if (status == null || (status.IsHiddenCustomer ?? true))
                return result.Init(message: "Không tìm thấy thông tin trạng thái");
            try
            {
                var cityId = poCurrent.CityId ?? 0;
                if (partnerId == (int)EMSHelper.PARTNER_ID)
                {
                    var EMSService = new EMSService();
                    var statusEMS = EMSHelper.ConvertToEMSStatus(lading.Status ?? 0, lading.Return ?? false);
                    var note = "";
                    if (string.IsNullOrEmpty(statusEMS))
                    {
                        note = status.StatusCustomerName;
                        if (status.StatusId == (int)StatusLading.DenTrungTamSanBayNhan || status.StatusId == (int)StatusLading.RoiTrungTamBay)
                            note += ". " + poCurrent.AirportName;
                    }
                    var city = _context.Locations.FirstOrDefault(x => x.LocationId == cityId);
                    var response = await EMSService.Delivery(new EMSLadingDeliveryRequest(lading.Code, statusEMS,
                                                city?.LocationEMSId + "", city?.PostOffficeEMSCode, datetime ?? DateTime.Now, note));
                    result.Data = response.Data;
                    result.Error = response.Error;
                    result.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public List<Lading> GetLadings(int offId, string code = null, int? status = null, DateTime? startTime = null, DateTime? endTime = null, int? pageNum = 0, int? pageSize = 20)
        {
            Expression<Func<Lading, bool>> expresionFinal = c => c.State == 0 && c.OfficerId == offId;

            if (status.HasValue)
            {
                Expression<Func<Lading, bool>> expresionId = c => (c.Status == status);
                expresionFinal = PredicateBuilder.And(expresionFinal, expresionId);
            }

            if (!String.IsNullOrWhiteSpace(code))
            {
                Expression<Func<Lading, bool>> expresionCode = c => (c.Code == code);
                expresionFinal = PredicateBuilder.And(expresionFinal, expresionCode);
            }
            if (endTime.HasValue && startTime.HasValue)
            {
                Expression<Func<Lading, bool>> expresionCode = c => (c.CreateDate.Value.Date >= startTime.Value.Date && c.CreateDate <= endTime.Value.Date);
                expresionFinal = PredicateBuilder.And(expresionFinal, expresionCode);
            }
            return _context.Ladings.Where(expresionFinal).Skip(pageNum.Value * pageSize.Value).Take(pageSize.Value).ToList();
        }
        public List<ReportAccumulateResponseModel> ReportAccumulate(int? customerId = null, DateTime? fromDate = null, DateTime? toDate = null, bool? isUsed = null, int? pageNumber = null, int? pageSize = null)
        {
            SqlParameter CustomerId = new SqlParameter("@CustomerId", customerId);
            if (!customerId.HasValue) CustomerId.Value = DBNull.Value;


            SqlParameter FromDate = new SqlParameter("@FromDate", fromDate);
            if (!fromDate.HasValue) FromDate.Value = DBNull.Value;

            SqlParameter ToDate = new SqlParameter("@ToDate", toDate);
            if (!toDate.HasValue) ToDate.Value = DBNull.Value;

            SqlParameter PageNumber = new SqlParameter("@PageNumber", pageNumber);
            if (!pageNumber.HasValue) PageNumber.Value = DBNull.Value;

            SqlParameter PageSize = new SqlParameter("@PageSize", pageSize);
            if (!pageSize.HasValue) PageSize.Value = DBNull.Value;

            SqlParameter IsUsed = new SqlParameter("@IsUsed", isUsed);
            if (!isUsed.HasValue) IsUsed.Value = DBNull.Value;

            List<SqlParameter> sqlParameters = new List<SqlParameter>();
            sqlParameters.Add(CustomerId);
            sqlParameters.Add(FromDate);
            sqlParameters.Add(ToDate);
            sqlParameters.Add(IsUsed);
            sqlParameters.Add(PageNumber);
            sqlParameters.Add(PageSize);
            return SqlHelper.ExecuteQuery<ReportAccumulateResponseModel>(_context, "Proc_ReportAccumulate", sqlParameters).ToList();
        }

    }

}