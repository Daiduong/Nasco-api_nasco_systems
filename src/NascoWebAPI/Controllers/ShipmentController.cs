﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NascoWebAPI.Data;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json.Linq;
using System.Reflection;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static NascoWebAPI.Helper.Common.Constants;
using System.Dynamic;
using System.Text;
using NascoWebAPI.Models;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json;charset=UTF-8")]
    [Route("api/Shipment")]
    [Authorize]
    public class ShipmentController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IRecipientRepository _recipientRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IPriceListRepository _priceListRepository;
        private readonly ILadingRepository _ladingRepository;
        private readonly IBKInternalRepository _bkInternalRepository;
        private readonly IBKInternalHistoryRepository _bkInternalHistoryRepository;
        private readonly IBKDeliveryRepository _bkDeliveryRepository;
        private readonly ILadingMapPriceRepository _ladingMapPriceRepository;
        private readonly ILadingTempRepository _ladingTempRepository;
        private readonly ITypeReasonRepository _typeReasonRepository;
        private readonly IProvideLadingRepository _provideLadingRepository;
        private readonly IAreaRepository _areaRepository;

        public ShipmentController(
            ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IBKInternalRepository bkInternalRepository,
             IBKDeliveryRepository bkDeliveryRepository,
             ICustomerRepository customerRepository,
             IPriceListRepository priceListRepository,
             IRecipientRepository recipientRepository,
             ILocationRepository locationRepository,
             IPostOfficeRepository postOfficeRepository,
             ILadingMapPriceRepository ladingMapPriceRepository,
             ILadingTempRepository ladingTempRepository,
             ITypeReasonRepository typeReasonRepository,
             IProvideLadingRepository provideLadingRepository,
             IAreaRepository areaRepository,
             IBKInternalHistoryRepository bkInternalHistoryRepository,
        ApplicationDbContext context) : base(logger)
        {
            _officeRepository = officeRepository;
            _bkInternalRepository = bkInternalRepository;
            _bkDeliveryRepository = bkDeliveryRepository;
            _customerRepository = customerRepository;
            _priceListRepository = priceListRepository;
            _recipientRepository = recipientRepository;
            _locationRepository = locationRepository;
            _postOfficeRepository = postOfficeRepository;
            _ladingMapPriceRepository = ladingMapPriceRepository;
            _ladingTempRepository = ladingTempRepository;
            _typeReasonRepository = typeReasonRepository;
            _provideLadingRepository = provideLadingRepository;
            _areaRepository = areaRepository;
            _bkInternalHistoryRepository = bkInternalHistoryRepository;
            _ladingRepository = new LadingRepository(context);
        }
        #region [Get]
        [HttpGet("IsExistedCode")]
        [AllowAnonymous]
        public JsonResult IsExistedCode(string code)
        {
            return Json(_ladingRepository.EqualsCode(code));
        }
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetAll(string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
                var result = await _ladingRepository.GetAsync(l => l.OfficerId == user.OfficerID, includeProperties: includeProperties);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetWaitingForTransfer")]
        public async Task<JsonResult> GetWaitingForTransfer(string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
                var result = await _ladingRepository.GetAsync(l => l.OfficerDelivery == user.OfficerID && (int)StatusLading.DangTrungChuyen == l.Status, includeProperties: includeProperties);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetByBKInternal")]
        public async Task<JsonResult> GetByBKInternal(string internalCode, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (!user.PostOfficeId.HasValue)
                {
                    return JsonError("Không tìm thấy Trạm của nhân viên");
                }
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
                var ladings = await _ladingRepository.GetAsync(l => l.Code == internalCode, includeProperties: includeProperties);
                if (ladings.Count() > 0)
                {
                    internalCode = ladings.First().KeyBk;
                }
                if (string.IsNullOrEmpty(internalCode))
                {
                    return JsonError("Không tìm thấy bảng kê");
                }
                var bkInternal = await _bkInternalRepository.GetFirstAsync(i => i.Code_BK_internal.Equals(internalCode));
                if (bkInternal == null || string.IsNullOrEmpty(bkInternal.ListLadingId) || bkInternal.ListLadingId.Replace("//", ";").Split(';').ToArray().Length == 0)
                {
                    return JsonError("Không tìm thấy bảng kê");
                }
                var statusLadingSuccess = (int)StatusLading.XuatKhoTrungChuyen;
                var statusLadingScanned = (int)StatusLading.DangTrungChuyen;
                var isFly = (bkInternal.IsFly ?? false);
                var poConfirm = bkInternal.POCreate.Value;
                var allowConfirm = true;//Được phép nhận hàng
                var confirmTo = false;//Xác nhận bởi trạm gửi
                var isDelivery = true;

                var listLadingID = bkInternal.ListLadingId.Replace("//", ";").Split(';').Select(long.Parse);
                ladings = await _ladingRepository.GetAsync(l => listLadingID.Contains(l.Id), includeProperties: includeProperties);

                switch (bkInternal.Status)
                {
                    case (int)StatusBK.DaTao:
                        bkInternal.Status = (int)StatusBK.TramGuiDangTrungChuyen;
                        break;
                    case (int)StatusBK.TramGuiDangTrungChuyen:
                        if (ladings.Count(o => o.Status == (int)StatusLading.XuatKhoTrungChuyen) > 0)
                        {

                        }
                        else
                        {
                            if (!isFly) allowConfirm = false;
                            else
                            {
                                isDelivery = false;
                                bkInternal.Status = (int)StatusBK.TramGuiBPSBXN;
                                statusLadingSuccess = (int)StatusLading.DangTrungChuyen;
                                statusLadingScanned = (int)StatusLading.SBXN;
                            }
                        }
                        break;
                    case (int)StatusBK.DenSanBay:
                        confirmTo = true;
                        poConfirm = bkInternal.PostOfficeId.Value;
                        isDelivery = false;
                        bkInternal.Status = (int)StatusBK.TramNhanBPSBXN;
                        if (ladings.Count(o => o.Status == (int)StatusLading.DangTrungChuyen) > 0)
                        {
                            statusLadingSuccess = (int)StatusLading.DangTrungChuyen;
                            statusLadingScanned = (int)StatusLading.SBXN;
                        }
                        else
                        {
                            statusLadingSuccess = (int)StatusLading.DenTrungTamSanBayNhan;
                            statusLadingScanned = (int)StatusLading.SBXN;
                        }

                        break;
                    case (int)StatusBK.TramNhanBPSBXN:
                        confirmTo = true;
                        poConfirm = bkInternal.PostOfficeId.Value;

                        bkInternal.Status = (int)StatusBK.TramNhanDangTrungChuyen;
                        if (ladings.Count(o => o.Status == (int)StatusLading.DenTrungTamSanBayNhan) > 0)
                        {
                            statusLadingSuccess = (int)StatusLading.DenTrungTamSanBayNhan;
                            statusLadingScanned = (int)StatusLading.SBXN;
                        }
                        else
                        {
                            statusLadingSuccess = (int)StatusLading.SBXN;
                            statusLadingScanned = (int)StatusLading.DangTrungChuyen;
                        }
                        break;
                    case (int)StatusBK.TramNhanDangTrungChuyen:
                        confirmTo = true;
                        poConfirm = bkInternal.PostOfficeId.Value;
                        bkInternal.Status = (int)StatusBK.TramNhanDangTrungChuyen;
                        if (ladings.Count(o => o.Status == (int)StatusLading.SBXN) > 0)
                        {
                            statusLadingSuccess = (int)StatusLading.SBXN;
                            statusLadingScanned = (int)StatusLading.DangTrungChuyen;
                        }
                        else
                        {
                            allowConfirm = false;
                        }
                        break;
                    default:
                        allowConfirm = false;
                        break;
                }
                if (!allowConfirm)
                {
                    return JsonError("Bảng kê không đúng trạng thái");
                }
                if (!(await _postOfficeRepository.SameCenter(poConfirm, user.PostOfficeId.Value)))
                {
                    return JsonError("Bảng kê này phải được xác nhận bởi nhân viên chịu sự quản lý của TRẠM " + (confirmTo ? "NHẬN" : "GỬI"));
                }
                if (!isDelivery)
                {
                    var po = await _postOfficeRepository.GetSingleAsync(o => o.PostOfficeID == user.PostOfficeId.Value);
                    if (po == null || !(po.IsCenter ?? false)
                    || !bkInternal.DepartmentConfirmId.HasValue || !bkInternal.JobConfirmId.HasValue || !user.DeparmentID.HasValue || !user.JobId.HasValue ||
                        (bkInternal.DepartmentConfirmId != user.DeparmentID && bkInternal.JobConfirmId != user.JobId))
                    {
                        return JsonError("Bảng kê này phải được xác nhận bởi BỘ PHẬN KHAI THÁC BAY của TRẠM " + (confirmTo ? "NHẬN" : "GỬI"));
                    }
                }
                else
                {
                    if (!_officeRepository.IsDelivery(user.OfficerID))
                    {
                        return JsonError("Bảng kê này phải được xác nhận bởi BỘ PHẬN GIAO NHẬN của TRẠM " + (confirmTo ? "NHẬN" : "GỬI"));
                    }
                }

                int succeeded = 0;
                int failed = 0;
                int scanned = 0;
                var ladingIdsSuccess = new List<long>();
                foreach (var lading in ladings)
                {
                    if (lading.Status == statusLadingSuccess)
                    {
                        lading.Status = statusLadingScanned;
                        lading.OfficerDelivery = user.OfficerID;
                        lading.ModifiedBy = user.OfficerID;
                        _ladingRepository.UpdateAndInsertLadingHistory(lading);
                        succeeded++;
                        ladingIdsSuccess.Add(lading.Id);
                    }
                    else if (lading.Status == statusLadingScanned)
                    {
                        scanned++;
                    }
                    else
                    {
                        failed++;
                    }
                }
                if (succeeded > 0)
                {
                    //if (!string.IsNullOrEmpty(bkInternal.LadingConfirmIds))
                    //{
                    //    var ids = bkInternal.LadingConfirmIds.Replace("//", ",").Split(',').Select(long.Parse).ToList().Union(ladingIdsSuccess);
                    //    bkInternal.LadingConfirmIds = string.Join("//", ids.Distinct());
                    //}
                    //else
                    //{
                    //    bkInternal.LadingConfirmIds = string.Join("//", ladingIdsSuccess);
                    //}
                    bkInternal.IsConfirmByOfficer = true;
                    bkInternal.OfficerConfirmId = user.OfficerID;
                    #region Thêm lịch sử bảng kê
                    var bkhistory = new BKInternalHistory
                    {
                        BK_internalId = bkInternal.ID_BK_internal,
                        CreatedBy = user.OfficerID,
                        CreatedDate = DateTime.Now,
                        LadingIds = string.Join("//", ladingIdsSuccess),
                        TotalLading = succeeded,
                        Status = bkInternal.Status,
                        Note = "Nhân viên xác nhận"
                    };
                    _bkInternalHistoryRepository.Insert(bkhistory);
                    #endregion
                    _bkInternalRepository.SaveChanges();
                }
                var result = new
                {
                    Success = succeeded,
                    Fail = failed,
                    Scanned = scanned
                };
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetByBKDelivery")]
        public async Task<JsonResult> GetByBKDelivery(string deliveryCode, string cols = "recipientid")
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
                var lading = await _ladingRepository.GetAsync(l => l.Code == deliveryCode, includeProperties: includeProperties);
                if (lading == null || lading.Count() == 0)
                {
                    var bkDelivery = await _bkDeliveryRepository.GetSingleAsync(i => i.Code_BK_Delivery.Equals(deliveryCode));
                    if (bkDelivery == null || bkDelivery.ListLadingId.Replace("//", ";").Split(';').ToArray().Length == 0)
                    {
                        return JsonError("BKDelivery Not Found");

                    }
                    var listLadingID = bkDelivery.ListLadingId.Replace("//", ";").Split(';');
                    var result = await _ladingRepository.GetAsync(l => listLadingID.Contains(l.Id.ToString()), includeProperties: includeProperties);
                    if (result == null)
                    {
                        return JsonError("Not Found");
                    }
                    return Json(result);
                }
                return Json(lading);
            }
            return JsonError("null");
        }
        [HttpGet("GetByStatus")]
        public async Task<JsonResult> GetByStatus(int statusID, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                Expression<Func<Lading, bool>> predicate = l => l.Status == statusID;
                if (statusID == (int)StatusLading.DangLayHang || statusID == (int)StatusLading.ChoLayHang)
                {
                    predicate = l => l.Status == statusID && l.OfficerPickup == user.OfficerID;
                }
                else if (statusID == (int)StatusLading.DangPhat || statusID == (int)StatusLading.XuatKho)
                {
                    predicate = l => l.Status == statusID && l.OfficerDelivery == user.OfficerID;
                }
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);

                var result = await _ladingRepository.GetAsync(predicate, includeProperties: includeProperties);

                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");

        }
        [HttpGet("GetByStatusLadingTemp")]
        public async Task<JsonResult> GetByStatusLadingTemp(int statusID, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                Expression<Func<LadingTemp, bool>> predicate = l => l.Status == statusID;
                if (statusID == (int)StatusLading.DangLayHang || statusID == (int)StatusLading.ChoLayHang)
                {
                    predicate = l => l.Status == statusID && l.OfficerPickup == user.OfficerID;
                }
                else if (statusID == (int)StatusLading.DangPhat || statusID == (int)StatusLading.XuatKho)
                {
                    predicate = l => l.Status == statusID && l.OfficerDelivery == user.OfficerID;
                }
                Expression<Func<LadingTemp, object>>[] includeProperties = getIncludeLadingTemp(cols);

                var result = await _ladingTempRepository.GetAsync(predicate, includeProperties: includeProperties);

                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");

        }
        [HttpGet("GetByGeoSame")]
        public async Task<JsonResult> GetByGeoSame(int id, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                var lading = await _ladingRepository.GetSingleAsync(l => l.Id == id && l.OfficerPickup == user.OfficerID);
                if (lading != null)
                {
                    Expression<Func<LadingTemp, object>>[] includeProperties = getIncludeLadingTemp(cols);
                    var result = await _ladingTempRepository.GetAsync(l => l.OfficerPickup == lading.OfficerPickup && l.LatFrom == lading.LatFrom && l.LngFrom == lading.LngFrom && (int)StatusLading.DangLayHang == l.Status, includeProperties: includeProperties);
                    if (result == null)
                    {
                        return JsonError("Same Geo: Not Found");
                    }
                    return Json(result);
                }
                return JsonError("Not Found");

            }
            return JsonError("null");

        }
        [HttpGet("GetSingle")]
        public async Task<JsonResult> GetSingle(string ladingCode, string cols = null)
        {
            Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
            var result = await _ladingRepository.GetSingleAsync(l => l.Code == ladingCode, includeProperties: includeProperties);
            if (result == null)
            {
                return JsonError("Not Found");
            }
            var ldHistory = await _ladingRepository.GetLastLadingHistoryAsync(result.Id);
            var jsonLading = Newtonsoft.Json.JsonConvert.SerializeObject(result,
                    Formatting.Indented,
    new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            var jsonLatLng = Newtonsoft.Json.JsonConvert.SerializeObject(new { latCurrent = ldHistory.Lat, lngCurrent = ldHistory.Lng });
            JObject jOLading = JObject.Parse(jsonLading);
            JObject jOLatLng = JObject.Parse(jsonLatLng);

            jOLading.Merge(jOLatLng, new JsonMergeSettings
            {
                MergeArrayHandling = MergeArrayHandling.Union
            });
            return Json(jOLading);
        }
        [HttpGet("GetSingleLadingTemp")]
        public async Task<JsonResult> GetSingleLadingTemp(string ladingCode, string cols = null)
        {
            Expression<Func<LadingTemp, object>>[] includeProperties = getIncludeLadingTemp(cols);
            var result = await _ladingTempRepository.GetSingleAsync(l => l.Code == ladingCode, includeProperties: includeProperties);
            if (result == null)
            {
                return JsonError("Not Found");
            }
            return Json(result);
        }
        [HttpGet("GetPickUp")]
        public async Task<JsonResult> GetPickUp(string cols = null)
        {
            Expression<Func<LadingTemp, object>>[] includeProperties = getIncludeLadingTemp(cols);
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                var result = await _ladingTempRepository.GetAsync(l => l.OfficerPickup == user.OfficerID && l.Status == (int)StatusLading.ChoLayHang, includeProperties: includeProperties);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetLadingReport")]
        public async Task<JsonResult> GetLadingReport(int reportType)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                var result = await _ladingRepository.GetLadingReport(user.OfficerID, reportType);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetSumLadingReport")]
        public async Task<JsonResult> GetSumLadingReport()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                List<dynamic> result = new List<dynamic>();
                foreach (ReportType rpt in Helper.Helper.GetValues<ReportType>())
                {
                    dynamic report = new ExpandoObject();
                    report.ReportType = (int)rpt;
                    report.ReportTypeName = Helper.Helper.GetEnumDescription(rpt);
                    report.Quantity = await _ladingRepository.GetSumLadingReport(user.OfficerID, (int)rpt);
                    result.Add(report);
                }
                return Json(result);
            }
            return JsonError("null");
        }


        #endregion

        #region [Insert - Update]
        [HttpPost("Insert")]
        public async Task<JsonResult> Insert([FromBody]LadingViewModel ladingModel)
        {
            if (ladingModel == null)
            {
                return JsonError("Lading not empty");
            }
            if (!string.IsNullOrEmpty(ladingModel.Code) && _ladingRepository.EqualsCode(ladingModel.Code))
            {
                return JsonError("Lading Code existed!");
            }
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Lading lading = new Lading();
                var pOId = await _officeRepository.GetPOIdByOffice(user.OfficerID) ?? 0;
                #region Ng??i g?i
                if (!ladingModel.SenderId.HasValue || ladingModel.SenderId <= 0)
                {
                    var customerExists = await _customerRepository.GetCustomerByPhone(ladingModel.SenderPhone);
                    if (customerExists == null)
                    {
                        Customer cus = new Customer();
                        cus.Address = ladingModel.SenderAddress;
                        cus.CompanyName = ladingModel.SenderCompany;
                        cus.Phone = ladingModel.SenderPhone;
                        cus.CustomerName = ladingModel.SenderName;
                        cus.State = 0;
                        cus.AddressNote = ladingModel.AddressNoteFrom;
                        cus.Create_Date = DateTime.Now;
                        cus.Create_By = user.OfficerID;
                        cus.Type = 0;//Khách vãng lai
                        cus.PostOffice_Id = pOId;
                        //Insert Customer
                        _customerRepository.Insert(cus);
                        _customerRepository.SaveChanges();
                        cus.CustomerCode = _customerRepository.GetCode(cus.CustomerID);
                        cus.PriceListID = await _priceListRepository.GetPriceApplyByPOID(pOId);
                        //UpdateCustomer
                        _customerRepository.SaveChanges();
                        lading.SenderId = cus.CustomerID;
                        ladingModel.SenderId = cus.CustomerID;
                    }
                    else
                    {
                        lading.SenderId = customerExists.CustomerID;
                        ladingModel.SenderId = customerExists.CustomerID;

                    }
                }
                else
                {
                    lading.SenderId = ladingModel.SenderId;
                }
                //TODO: Thêm khách l? + ??a ch?
                #endregion

                #region Ng??i nh?n

                var recipient = new Recipient();
                //if (ladingModel.RecipientId != null && ladingModel.RecipientId > 0)
                //{
                //    var dataR = unitOfWork.RecipientRepository.GetById(ladingModel.RecipientId.Value);
                //    recipient = dataR ?? new Recipient();
                //}
                if (ladingModel.RecipientPhone != string.Empty)
                {
                    var dataR = await _recipientRepository.GetFirstAsync(o => o.Phone == ladingModel.RecipientPhone);
                    recipient = dataR ?? new Recipient();

                    recipient.State = (int)StatusSystem.Enable;
                    recipient.Address = ladingModel.RecipientAddress;
                    recipient.Name = ladingModel.RecipientName;
                    recipient.Phone = ladingModel.RecipientPhone;
                    recipient.CompanyName = ladingModel.RecipientCompany;
                    recipient.AddressNote = ladingModel.AddressNoteTo;
                    //Insert recipient
                    if (dataR != null)
                    {
                        _recipientRepository.Update(recipient);

                    }
                    else
                    {
                        _recipientRepository.Insert(recipient);
                    }
                    _recipientRepository.SaveChanges();
                    lading.RecipientId = recipient.Id;
                }
                #endregion

                #region Thông tin

                lading.PartnerCode = ladingModel.PartnerCode;
                lading.State = (int)StatusSystem.Enable;
                if (ladingModel.Id <= 0)
                {
                    lading.CreateDate = DateTime.Now;
                    ladingModel.OficerId = user.OfficerID;
                    lading.OfficerId = ladingModel.OficerId;
                    lading.OfficerPickup = ladingModel.OfficerPickup;
                    lading.POCurrent = pOId;
                }
                if (ladingModel.Status == 1)
                {
                    lading.Status = (int)StatusLading.KHTaoBill;
                }
                else
                {
                    lading.Status = (int)StatusLading.DaLayHang;
                }
                lading.TransportID = ladingModel.TransportID;
                lading.StructureID = ladingModel.StructureID;
                lading.RDFrom = ladingModel.RDFrom;
                lading.RDTo = ladingModel.RDTo;
                lading.IsGlobal = ladingModel.IsGlobal;
                lading.CitySendId = ladingModel.CitySendId;
                lading.DistrictFrom = ladingModel.DistrictFrom;
                lading.CityRecipientId = ladingModel.CityRecipientId;
                lading.DistrictTo = ladingModel.DistrictTo;
                lading.AddressNoteTo = ladingModel.AddressNoteTo;
                lading.AddressNoteFrom = ladingModel.AddressNoteFrom;
                //lading.POFrom = ladingModel.POFrom;
                //lading.CenterFrom = ladingModel.CenterFrom;
                //lading.POTo = ladingModel.POTo;
                //lading.CenterTo = ladingModel.CenterTo;
                #region [Set PostOffice] 
                int? poFrom = ladingModel.POFrom;
                int? poTo = ladingModel.POTo;
                if ((poFrom ?? 0) <= 0)
                {
                    var po = await _postOfficeRepository.GetDistanceMinByLocation(lading.CitySendId ?? 0, ladingModel.LatFrom ?? 0, ladingModel.LngFrom ?? 0, 1);
                    poFrom = po.PostOfficeID;
                }
                if ((poTo ?? 0) <= 0)
                {
                    var po = await _postOfficeRepository.GetDistanceMinByLocation(lading.CityRecipientId ?? 0, ladingModel.LatTo ?? 0, ladingModel.LngTo ?? 0, 2);
                    poTo = po.PostOfficeID;
                }
                lading.POCreated = pOId;
                lading.POCurrent = pOId;
                lading.POFrom = poFrom;
                lading.POTo = poTo;
                if (_postOfficeRepository.Any(o => o.PostOfficeID == lading.POFrom))
                {
                    lading.CenterFrom = (await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == lading.POFrom)).POCenterID ?? lading.POFrom;
                }
                if (_postOfficeRepository.Any(o => o.PostOfficeID == lading.POTo))
                {
                    lading.CenterTo = (await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == lading.POTo)).POCenterID ?? lading.POTo;
                }
                #endregion
                lading.AddressFrom = ladingModel.AddressFrom;
                lading.AddressTo = ladingModel.AddressTo;
                lading.NameFrom = ladingModel.NameFrom;
                lading.NameTo = ladingModel.NameTo;
                lading.PhoneFrom = ladingModel.PhoneFrom;
                lading.PhoneTo = ladingModel.PhoneTo;
                lading.CompanyFrom = ladingModel.CompanyFrom;
                lading.CompanyTo = ladingModel.CompanyTo;
                //
                lading.LatFrom = ladingModel.LatFrom;
                lading.LngFrom = ladingModel.LngFrom;
                lading.LatTo = ladingModel.LatTo;
                lading.LngTo = ladingModel.LngTo;
                lading.RouteLength = ladingModel.RouteLength;
                lading.Weight = ladingModel.Weight;
                lading.Id = ladingModel.Id;
                lading.Width = ladingModel.Width;
                lading.Height = ladingModel.Height;
                lading.Length = ladingModel.Length;
                lading.Number = ladingModel.Number ?? 1;
                if (lading.Number == 0) lading.Number = 1;
                lading.Mass = ladingModel.Mass;
                lading.Noted = ladingModel.Noted;
                lading.Description = ladingModel.Description;
                lading.OnSiteDeliveryPrice = ladingModel.OnSiteDeliveryPrice;
                #endregion

                #region D?ch v? + Thanh toán

                lading.ServiceId = ladingModel.ServiceId;
                lading.PaymentType = ladingModel.PaymentId;
                if (lading.PaymentType.HasValue && lading.PaymentType == (int)PaymentType.Done)
                {
                    lading.PaymentAmount = true;
                }
                #endregion
                if (ladingModel.COD > 0)
                {
                    lading.StatusCOD = (int)StatusCOD.ChuaThu;
                }
                else
                {
                    lading.StatusCOD = (int)StatusCOD.KhongCOD;
                }
                #region Dịch vụ giá trị gia tăng
                lading.COD = ladingModel.COD;
                lading.CODPrice = ladingModel.CODPrice;
                lading.PPXDPercent = ladingModel.PPXDPercent;
                lading.Insured = ladingModel.Insured;
                lading.InsuredPrice = ladingModel.InsuredPrice;
                lading.TotalPriceDVGT = ladingModel.TotalPriceDVGT;
                lading.PriceVAT = ladingModel.PriceVAT;
                lading.NoteTypePack = ladingModel.NoteTypePack;
                lading.PackPrice = ladingModel.PackPrice;
                lading.PackId = ladingModel.PackId;//Loại đóng gói
                lading.PriceOther = ladingModel.PriceOther;
                lading.InsuredPercent = ladingModel.InsuredPercent;
                lading.KDPrice = ladingModel.KDPrice;
                lading.KDNumber = ladingModel.KDNumber;
                #endregion

                #region Thông tin kiên
                if (ladingModel.Number_L_W_H_DIM_List != null && ladingModel.Number_L_W_H_DIM_List.Count > 0)
                {
                    lading.Number_L_W_H_DIM = string.Join(",", ladingModel.Number_L_W_H_DIM_List.Where(o => o.Number > 0 && o.DIM > 0).Select(o => o.Number + "_" + o.Long + "_" + o.Width + "_" + o.Height + "_" + o.DIM));
                }
                #endregion
                lading.OfficerId = user.OfficerID;
                lading.ModifiedBy = user.OfficerID;
                lading.PriceListId = ladingModel.PriceListId;
                if (lading.PaymentType.HasValue && lading.PaymentType == (int)PaymentType.Done)
                {
                    lading.PaymentAmount = true;
                }
                #region Giá + Lưu
                if (_ladingRepository.EqualsCode(ladingModel.Code) == false)
                {
                    lading.Code = ladingModel.Code;
                    //C?p nh?p c?p v?n ??n
                    var provideLading = await _provideLadingRepository.GetFirstAsync(o => o.LadingCode.ToUpper().Equals(ladingModel.Code.ToUpper()));
                    if (provideLading != null && provideLading.Status == 201)
                    {
                        provideLading.Status = 202;
                        _provideLadingRepository.Update(provideLading);
                    }

                }
                else return JsonError("Mã vận đơn đã tồn tại");
                if (lading.OfficerId != null && lading.OfficerId > 0)
                {
                    var postOffice = await _postOfficeRepository.GetSingleAsync( o=> o.PostOfficeID == user.PostOfficeId.Value );
                    if (postOffice != null && postOffice.PostOfficeID > 0)
                    {
                        lading.IsPriceMain = ladingModel.IsPriceMain;
                        lading.PriceMain = ladingModel.PriceMain;
                        lading.Amount = ladingModel.Amount;
                        _ladingRepository.InsertAndInsertLadingHistory(lading);
                        if (string.IsNullOrEmpty(lading.Code))
                        {
                            lading.Code = _ladingRepository.CodeGenerationByLocationCode(postOffice.LocationId ?? 0, (int)lading.Id);
                        }
                        if (ladingModel.AnotherServiceId != null)
                        {
                            foreach (var serviceId in ladingModel.AnotherServiceIds)
                            {
                                var serviceMap = new LadingMapService { LadingId = lading.Id, ServiceId = serviceId };
                                //Insert serviceMap
                                _ladingMapPriceRepository.Insert(serviceMap);
                            }
                            _ladingMapPriceRepository.SaveChanges();
                        }

                    }
                    else
                    {
                        return JsonError("Không tìm thấy thông tin bưu cục của nhân viên");
                    }
                }
                #endregion
                _logger.LogInformation($"Insert ShipmentID {lading.Id}: user {user.UserName}");
                return Json(lading);
            }
            return JsonError("null");
        }
        [HttpPost("UpdateLading")]
        public async Task<JsonResult> UpdateLading([FromBody]LadingViewModel ladingModel)
        {
            if (ladingModel == null)
            {
                return JsonError("dữ liệu đẩy vào không được trống");
            }
            if (!_ladingTempRepository.Any(o => o.Id == ladingModel.Id))
            {
                return JsonError("Không tìm thấy thông tin yêu cầu");
            }
            if (!string.IsNullOrEmpty(ladingModel.Code) && _ladingRepository.EqualsCode(ladingModel.Code))
            {
                return JsonError("Mã vận đơn đã tồn tại!");
            }
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                LadingTemp lading = new LadingTemp();
                lading = await _ladingTempRepository.GetFirstAsync(o => o.Id == ladingModel.Id);
                var pOId = await _officeRepository.GetPOIdByOffice(user.OfficerID) ?? 0;
                #region Ng??i g?i
                if (!ladingModel.SenderId.HasValue || ladingModel.SenderId <= 0)
                {
                    var customerExists = await _customerRepository.GetCustomerByPhone(ladingModel.SenderPhone);
                    if (customerExists == null)
                    {
                        Customer cus = new Customer();
                        cus.Address = ladingModel.SenderAddress;
                        cus.CompanyName = ladingModel.SenderCompany;
                        cus.Phone = ladingModel.SenderPhone;
                        cus.CustomerName = ladingModel.SenderName;
                        cus.State = 0;
                        cus.AddressNote = ladingModel.AddressNoteFrom;
                        cus.Create_Date = DateTime.Now;
                        cus.Create_By = user.OfficerID;
                        cus.Type = 0;//Khách vãng lai
                        cus.PostOffice_Id = pOId;
                        //Insert Customer
                        _customerRepository.Insert(cus);
                        _customerRepository.SaveChanges();
                        cus.CustomerCode = _customerRepository.GetCode(cus.CustomerID);
                        cus.PriceListID = await _priceListRepository.GetPriceApplyByPOID(pOId);
                        //UpdateCustomer
                        _customerRepository.SaveChanges();
                        lading.SenderId = cus.CustomerID;
                        ladingModel.SenderId = cus.CustomerID;
                    }
                    else
                    {
                        lading.SenderId = customerExists.CustomerID;
                        ladingModel.SenderId = customerExists.CustomerID;

                    }
                }
                else
                {
                    lading.SenderId = ladingModel.SenderId;
                }
                //TODO: Thêm khách l? + ??a ch?
                #endregion

                #region Ng??i nh?n

                var recipient = new Recipient();
                //if (ladingModel.RecipientId != null && ladingModel.RecipientId > 0)
                //{
                //    var dataR = unitOfWork.RecipientRepository.GetById(ladingModel.RecipientId.Value);
                //    recipient = dataR ?? new Recipient();
                //}
                if (ladingModel.RecipientPhone != string.Empty)
                {
                    var dataR = await _recipientRepository.GetFirstAsync(o => o.Phone == ladingModel.RecipientPhone);
                    recipient = dataR ?? new Recipient();

                    recipient.State = (int)StatusSystem.Enable;
                    recipient.Address = ladingModel.RecipientAddress;
                    recipient.Name = ladingModel.RecipientName;
                    recipient.Phone = ladingModel.RecipientPhone;
                    recipient.CompanyName = ladingModel.RecipientCompany;
                    recipient.AddressNote = ladingModel.AddressNoteTo;
                    //Insert recipient
                    if (dataR != null)
                    {
                        _recipientRepository.Update(recipient);

                    }
                    else
                    {
                        _recipientRepository.Insert(recipient);
                    }
                    _recipientRepository.SaveChanges();
                    lading.RecipientId = recipient.Id;
                }
                #endregion

                #region Thông tin

                lading.PartnerCode = ladingModel.PartnerCode;
                lading.State = (int)StatusSystem.Enable;
                if (ladingModel.Id <= 0)
                {
                    lading.CreateDate = DateTime.Now;
                    ladingModel.OficerId = user.OfficerID;
                    lading.OfficerId = user.OfficerID;
                    lading.OfficerPickup = user.OfficerID;
                    lading.POCurrent = pOId;
                }
                //lading.Status = ladingModel.Status;
                if (lading.Status == (int)StatusLading.DangLayHang || ladingModel.Status == (int)StatusLading.DaLayHang)
                {
                    lading.Status = (int)StatusLading.DaLayHang;
                }
                lading.TransportID = ladingModel.TransportID;
                lading.StructureID = ladingModel.StructureID;
                lading.RDFrom = ladingModel.RDFrom;
                lading.RDTo = ladingModel.RDTo;
                lading.IsGlobal = ladingModel.IsGlobal;
                lading.CitySendId = ladingModel.CitySendId;
                lading.DistrictFrom = ladingModel.DistrictFrom;
                lading.CityRecipientId = ladingModel.CityRecipientId;
                lading.DistrictTo = ladingModel.DistrictTo;
                lading.AddressNoteTo = ladingModel.AddressNoteTo;
                lading.AddressNoteFrom = ladingModel.AddressNoteFrom;
                #region [Set PostOffice] 
                int? poFrom = lading.POFrom;
                int? poTo = ladingModel.POTo;

                if ((poFrom ?? 0) <= 0)
                {
                    var po = await  _postOfficeRepository.GetDistanceMinByLocation(lading.CitySendId ?? 0, ladingModel.LatFrom ?? 0, ladingModel.LngFrom ?? 0, 1);
                    poFrom = po.PostOfficeID;
                }
                if ((poTo ?? 0) <= 0)
                {
                    var po = await _postOfficeRepository.GetDistanceMinByLocation(lading.CityRecipientId ?? 0, ladingModel.LatTo ?? 0, ladingModel.LngTo ?? 0, 2);
                    poTo = po.PostOfficeID;
                }
                lading.POCurrent = pOId;
                lading.POFrom = poFrom;
                if (!lading.POCreated.HasValue)
                {
                    lading.POCreated = pOId;
                }
                lading.POTo = poTo;
                if (_postOfficeRepository.Any(o => o.PostOfficeID == lading.POFrom))
                {
                    lading.CenterFrom = (await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == lading.POFrom)).POCenterID ?? lading.POFrom;
                }
                if (_postOfficeRepository.Any(o => o.PostOfficeID == lading.POTo))
                {
                    lading.CenterTo = (await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == lading.POTo)).POCenterID ?? lading.POTo;
                }
                #endregion
                lading.AddressFrom = ladingModel.AddressFrom;
                lading.AddressTo = ladingModel.AddressTo;
                lading.NameFrom = ladingModel.NameFrom;
                lading.NameTo = ladingModel.NameTo;
                lading.PhoneFrom = ladingModel.PhoneFrom;
                lading.PhoneTo = ladingModel.PhoneTo;
                lading.CompanyFrom = ladingModel.CompanyFrom;
                lading.CompanyTo = ladingModel.CompanyTo;
                //
                lading.LatFrom = ladingModel.LatFrom;
                lading.LngFrom = ladingModel.LngFrom;
                lading.LatTo = ladingModel.LatTo;
                lading.LngTo = ladingModel.LngTo;
                lading.RouteLength = ladingModel.RouteLength;
                lading.Weight = ladingModel.Weight;
                lading.Id = ladingModel.Id;
                lading.Width = ladingModel.Width;
                lading.Height = ladingModel.Height;
                lading.Length = ladingModel.Length;
                lading.Number = ladingModel.Number ?? 1;
                if (lading.Number == 0) lading.Number = 1;
                lading.Mass = ladingModel.Mass;
                lading.Noted = ladingModel.Noted;
                lading.Description = ladingModel.Description;
                lading.OnSiteDeliveryPrice = ladingModel.OnSiteDeliveryPrice;
                #endregion

                #region D?ch v? + Thanh toán

                lading.ServiceId = ladingModel.ServiceId;
                lading.PaymentType = ladingModel.PaymentId;

                #endregion
                if (ladingModel.COD > 0)
                {
                    lading.StatusCOD = (int)StatusCOD.ChuaThu;
                }
                else
                {
                    lading.StatusCOD = (int)StatusCOD.KhongCOD;
                }
                #region Dịch vụ giá trị gia tăng
                lading.COD = ladingModel.COD;
                lading.CODPrice = ladingModel.CODPrice;
                lading.PPXDPercent = ladingModel.PPXDPercent;
                lading.Insured = ladingModel.Insured;
                lading.InsuredPrice = ladingModel.InsuredPrice;
                lading.TotalPriceDVGT = ladingModel.TotalPriceDVGT;
                lading.PriceVAT = ladingModel.PriceVAT;
                lading.NoteTypePack = ladingModel.NoteTypePack;
                lading.PackPrice = ladingModel.PackPrice;
                lading.PackId = ladingModel.PackId;//Loại đóng gói
                lading.PriceOther = ladingModel.PriceOther;
                lading.InsuredPercent = ladingModel.InsuredPercent;
                lading.KDPrice = ladingModel.KDPrice;
                lading.KDNumber = ladingModel.KDNumber;
                #endregion
                #region Thông tin kiên
                if (ladingModel.Number_L_W_H_DIM_List != null && ladingModel.Number_L_W_H_DIM_List.Count > 0)
                {
                    lading.Number_L_W_H_DIM = string.Join(",", ladingModel.Number_L_W_H_DIM_List.Where(o => o.Number > 0 && o.DIM > 0).Select(o => o.Number + "_" + o.Long + "_" + o.Width + "_" + o.Height + "_" + o.DIM));
                }
                #endregion
                lading.ModifiedBy = user.OfficerID;
                lading.PriceListId = ladingModel.PriceListId;

                #region Giá + L?u
                var postOffice = await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == lading.POFrom.Value);
                if (postOffice != null && postOffice.PostOfficeID > 0)
                {
                    lading.IsPriceMain = ladingModel.IsPriceMain;
                    lading.PriceMain = ladingModel.PriceMain;
                    lading.Amount = ladingModel.Amount;
                    lading.AnotherServiceIds = ladingModel.AnotherServiceId;
                    _ladingTempRepository.Update(lading);//UpdateLading và insert hành trình
                    _ladingTempRepository.SaveChanges();
                    #region Thêm vận đơn Lading
                    if (lading.Status == (int)StatusLading.DaLayHang && (!lading.LadingId.HasValue || !_ladingRepository.Any(o => o.Id == lading.LadingId.Value)))
                    {
                        //Thêm m?i vào Lading
                        var ladingReal = new Lading();
                        var jsonLadingTemp = Newtonsoft.Json.JsonConvert.SerializeObject(lading,
                Formatting.Indented,
                        new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

                        if (ladingReal.CopyFromJOject<Lading>(JObject.Parse(jsonLadingTemp)))
                        {
                            ladingReal.Id = 0;
                            ladingReal.OfficerId = user.OfficerID;
                            ladingReal.OfficerPickup = user.OfficerID;
                            
                            if (ladingReal.PaymentType.HasValue && ladingReal.PaymentType == (int)PaymentType.Done)
                            {
                                ladingReal.PaymentAmount = true;
                            }
                            _ladingRepository.InsertAndInsertLadingHistory(ladingReal);
                            _ladingRepository.SaveChanges();
                            if (string.IsNullOrEmpty(ladingModel.Code))
                            {
                                ladingReal.Code = _ladingRepository.CodeGenerationByLocationCode(postOffice.LocationId ?? 0, (int)ladingReal.Id);
                            }
                            else
                            {
                                ladingReal.Code = ladingModel.Code;
                                //Cập nhập cấp vận đơn
                                var provideLading = await _provideLadingRepository.GetFirstAsync(o => o.LadingCode.ToUpper().Equals(ladingModel.Code.ToUpper()));
                                if (provideLading != null && provideLading.Status == 201)
                                {
                                    provideLading.Status = 202;
                                    _provideLadingRepository.Update(provideLading);
                                }
                            }
                            lading.LadingId = (int)ladingReal.Id;
                            if (ladingModel.AnotherServiceId != null)
                            {
                                foreach (var serviceId in ladingModel.AnotherServiceIds)
                                {
                                    var serviceMap = new LadingMapService { LadingId = lading.Id, ServiceId = serviceId };
                                    //Insert serviceMap
                                    _ladingMapPriceRepository.Insert(serviceMap);
                                }
                                _ladingMapPriceRepository.SaveChanges();
                            }
                            _ladingTempRepository.SaveChanges();
                            return Json(ladingReal);
                        }
                    }
                    #endregion

                }
                else
                {
                    return JsonError("Không tìm thấy thông tin trạm gửi");
                }

                #endregion
                _logger.LogInformation($"Update LadingTempID {lading.Id}: user {user.UserName}");
                return Json(lading);
            }
            return JsonError("null");
        }
        [HttpPost("Update")]
        public async Task<JsonResult> Update([FromBody]JObject jsonData)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null && jsonData != null)
            {
                dynamic json = jsonData;
                if (jsonData["id"] != null)
                {
                    int id = json.id;
                    if (_ladingRepository.Any(l => l.Id == id))
                    {
                        Lading lading = _ladingRepository.GetSingle(l => l.Id == id);
                        if (lading.CopyFromJOject(jsonData))
                        {
                            lading.ModifiedBy = user.OfficerID;
                            if ((int)StatusLading.DangLayHang == lading.Status)
                            {
                                if (lading.OfficerPickup != user.OfficerID)
                                {
                                    return JsonError(null);
                                }
                                lading.OfficerPickup = user.OfficerID;
                            }
                            else if ((int)StatusLading.XuatKho == lading.Status || (int)StatusLading.DangPhat == lading.Status)
                            {
                                lading.OfficerDelivery = user.OfficerID;
                            }
                            if (_ladingRepository.Any(l => l.Id == lading.Id && l.Status == lading.Status))
                            {
                                _ladingRepository.Update(lading);
                                _ladingRepository.SaveChanges();
                            }
                            else
                            {
                                string location = (string)json.location;

                                if (lading.Status == (int)StatusLading.NVKhongNhan || lading.Status == (int)StatusLading.LayHangKhongTC ||
                                    lading.Status == (int)StatusLading.PhatKhongTC)
                                {
                                    int? typeReasonID = (int?)json.typeReasonID;
                                    _ladingRepository.UpdateAndInsertLadingHistory(lading, typeReasonID, location);
                                }
                                else
                                {
                                    _ladingRepository.UpdateAndInsertLadingHistory(lading, null, location);
                                }
                            }

                            _logger.LogInformation($"Update ShipmentID {lading.Id}: user {user.UserName}");
                            return Json(lading);
                        }

                    }
                    return JsonError("Lading Not Found");
                }
                return JsonError("Id invalid or empty");


            }
            return JsonError("null");
        }
        [HttpPost("UpdateLadingTemp")]
        public async Task<JsonResult> UpdateLadingTemp([FromBody]JObject jsonData)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null && jsonData != null)
            {
                dynamic json = jsonData;
                if (jsonData["id"] != null)
                {
                    int id = json.id;
                    if (_ladingTempRepository.Any(l => l.Id == id))
                    {
                        var lading = _ladingTempRepository.GetSingle(l => l.Id == id);
                        if (lading.CopyFromJOject(jsonData))
                        {
                            lading.ModifiedBy = user.OfficerID;
                            if ((int)StatusLading.DangLayHang == lading.Status)
                            {
                                //if (lading.OfficerPickup != user.OfficerID)
                                //{
                                //    return JsonError(null);
                                //}
                                lading.OfficerPickup = user.OfficerID;
                            }
                            else if (lading.Status == (int)StatusLading.NVKhongNhan || lading.Status == (int)StatusLading.LayHangKhongTC)
                            {
                                int? typeReasonID = (int?)json.typeReasonID;
                                if (typeReasonID.HasValue)
                                {
                                    if (string.IsNullOrEmpty(lading.Noted))
                                    {
                                        var typeReson = _typeReasonRepository.GetSingle(obj => typeReasonID.Value == obj.TypeReasonID);
                                        if (typeReson != null)
                                        {
                                            lading.Noted = typeReson.TypeReasonName;
                                        }
                                    }
                                }

                            }
                            _ladingTempRepository.Update(lading);
                            _ladingTempRepository.SaveChanges();
                            _logger.LogInformation($"Update LadingTempId {lading.Id}: user {user.UserName}");
                            return Json(lading);
                        }

                    }
                    return JsonError("Lading Temp  Not Found");
                }
                return JsonError("Id invalid or empty");


            }
            return JsonError("null");
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            _ladingRepository.Dispose();
            base.Dispose(disposing);
        }

        public Expression<Func<Lading, object>>[] getInclude(string cols)
        {
            List<Expression<Func<Lading, object>>> includeProperties = new List<Expression<Func<Lading, object>>>();
            if (!string.IsNullOrEmpty(cols))
            {
                foreach (var col in cols.Split(','))
                {
                    var colValue = col.Trim().ToLower();
                    if (colValue == "recipientid")
                    {
                        includeProperties.Add(inc => inc.Recipient);
                    }
                    else if (colValue == "senderid")
                    {
                        includeProperties.Add(inc => inc.Sender);
                    }
                    else if (colValue == "serviceid")
                    {
                        includeProperties.Add(inc => inc.Service);
                    }
                    else if (colValue == "citysendid")
                    {
                        includeProperties.Add(inc => inc.CitySend);
                    }
                    else if (colValue == "cityrecipientid")
                    {
                        includeProperties.Add(inc => inc.CityRecipient);
                    }
                    else if (colValue == "status")
                    {
                        includeProperties.Add(inc => inc.CurrenSttStatus);
                    }
                    else if (colValue == "districtfrom")
                    {
                        includeProperties.Add(inc => inc.DistrictFromObj);
                    }
                    else if (colValue == "districtto")
                    {
                        includeProperties.Add(inc => inc.DistrictToObj);
                    }
                    else if (colValue == "transport")
                    {
                        includeProperties.Add(inc => inc.Transport);
                    }
                }
            }
            return includeProperties.ToArray();
        }
        public Expression<Func<LadingTemp, object>>[] getIncludeLadingTemp(string cols)
        {
            List<Expression<Func<LadingTemp, object>>> includeProperties = new List<Expression<Func<LadingTemp, object>>>();
            if (!string.IsNullOrEmpty(cols))
            {
                foreach (var col in cols.Split(','))
                {
                    var colValue = col.Trim().ToLower();
                    if (colValue == "recipientid")
                    {
                        includeProperties.Add(inc => inc.Recipient);
                    }
                    else if (colValue == "senderid")
                    {
                        includeProperties.Add(inc => inc.Sender);
                    }
                    else if (colValue == "serviceid")
                    {
                        includeProperties.Add(inc => inc.Service);
                    }
                    else if (colValue == "citysendid")
                    {
                        includeProperties.Add(inc => inc.CitySend);
                    }
                    else if (colValue == "cityrecipientid")
                    {
                        includeProperties.Add(inc => inc.CityRecipient);
                    }
                    else if (colValue == "status")
                    {
                        includeProperties.Add(inc => inc.CurrenSttStatus);
                    }
                    else if (colValue == "districtfrom")
                    {
                        includeProperties.Add(inc => inc.DistrictFromObj);
                    }
                    else if (colValue == "districtto")
                    {
                        includeProperties.Add(inc => inc.DistrictToObj);
                    }
                    else if (colValue == "transport")
                    {
                        includeProperties.Add(inc => inc.Transport);
                    }
                }
            }
            return includeProperties.ToArray();
        }

    }
}