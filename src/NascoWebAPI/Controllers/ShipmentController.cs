using System;
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
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using NascoWebAPI.Services;
using Microsoft.AspNetCore.Hosting;

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
        private readonly ILadingMapServiceRepository _ladingMapServiceRepository;
        private readonly ILadingTempRepository _ladingTempRepository;
        private readonly ITypeReasonRepository _typeReasonRepository;
        private readonly IProvideLadingRepository _provideLadingRepository;
        private readonly IAreaRepository _areaRepository;
        private readonly ICouponRepository _couponRepository;
        private readonly IPackageOfLadingRepository _packageOfLadingRepository;
        private readonly IPriceRepository _priceRepository;
        private readonly IEMSService _iEMSService;
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        public ShipmentController(
            ILogger<UserController> logger,
            ILadingRepository ladingRepository,
            IOfficerRepository officeRepository,
            IBKInternalRepository bkInternalRepository,
             IBKDeliveryRepository bkDeliveryRepository,
             ICustomerRepository customerRepository,
             IPriceListRepository priceListRepository,
             IRecipientRepository recipientRepository,
             ILocationRepository locationRepository,
             IPostOfficeRepository postOfficeRepository,
             ILadingMapServiceRepository ladingMapServiceRepository,
             ILadingTempRepository ladingTempRepository,
             ITypeReasonRepository typeReasonRepository,
             IProvideLadingRepository provideLadingRepository,
             IAreaRepository areaRepository,
             IBKInternalHistoryRepository bkInternalHistoryRepository,
             ICouponRepository couponRepository,
             IPackageOfLadingRepository packageOfLadingRepository,
             IPriceRepository priceRepository,
             IEMSService iEMSService,
             IHostingEnvironment env,
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
            _ladingMapServiceRepository = ladingMapServiceRepository;
            _ladingTempRepository = ladingTempRepository;
            _typeReasonRepository = typeReasonRepository;
            _provideLadingRepository = provideLadingRepository;
            _areaRepository = areaRepository;
            _bkInternalHistoryRepository = bkInternalHistoryRepository;
            _couponRepository = couponRepository;
            _packageOfLadingRepository = packageOfLadingRepository;
            _priceRepository = priceRepository;
            _iEMSService = iEMSService;
            _context = context;
            _ladingRepository = ladingRepository;
            _env = env;
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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
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
                if (string.IsNullOrEmpty(internalCode))
                {
                    return JsonError("Không tìm thấy bảng kê");
                }
                var result = await _bkInternalRepository.Transporting(user.OfficerID, internalCode);
                return result.Error == 0 ? Json(result.Data) : JsonError(result.Message);
            }
            return JsonError("null");
        }
        [HttpGet("GetByBKDelivery")]
        public async Task<JsonResult> GetByBKDelivery(string deliveryCode, string cols = "recipientid")
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
                if (string.IsNullOrEmpty(deliveryCode))
                {
                    return JsonError("Mã bảng kê không được bỏ trống");
                }
                var result = await _bkDeliveryRepository.Delivering(user.OfficerID, deliveryCode);
                return result.Error == 0 ? JsonSuccess(message: result.Message) : JsonError(result.Message);
            }
            return JsonError("null");
        }
        [HttpGet("GetByStatus")]
        public async Task<JsonResult> GetByStatus(int statusID, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                Expression<Func<Lading, bool>> predicate = l => l.Status == statusID;
                Expression<Func<PackageOfLading, bool>> predicatePOL = l => l.StatusId == statusID;
                if (statusID == (int)StatusLading.DangLayHang || statusID == (int)StatusLading.ChoLayHang || statusID == (int)StatusLading.DaLayHang)
                {
                    predicate = l => l.Status == statusID && l.OfficerPickup == user.OfficerID;
                    predicatePOL = l => l.Id == 0;
                }
                else if (statusID == (int)StatusLading.DangPhat || statusID == (int)StatusLading.XuatKho || statusID == (int)StatusLading.PhatKhongTC)
                {
                    predicate = l => l.Status == statusID && l.OfficerDelivery == user.OfficerID;
                    predicatePOL = l => l.StatusId == statusID && l.DeliveryBy == user.OfficerID;
                }
                else if (statusID == (int)StatusLading.DangTrungChuyen)
                {
                    predicate = l => l.Status == statusID && l.OfficerTransferId == user.OfficerID;
                }
                var ladingIds = _context.PackageOfLadings
                    .Where(predicatePOL.And(x => x.LadingId.HasValue))
                    .Select(x => x.LadingId.Value);
                if (ladingIds.Count() > 0)
                {
                    predicate = predicate.Or(x => ladingIds.Contains((int)x.Id));
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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                statusID = Helper.Helper.GetStatusLadingTemp(statusID);
                Expression<Func<LadingTemp, bool>> predicate = l => l.Status == statusID;
                if (statusID == (int)StatusLadingTemp.PickingUp || statusID == (int)StatusLadingTemp.WaitingPickUp || statusID == (int)StatusLadingTemp.PickedUp)
                {
                    predicate = l => l.Status == statusID && l.OfficerPickup == user.OfficerID;
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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                var lading = await _ladingRepository.GetFirstAsync(l => l.Id == id && l.OfficerPickup == user.OfficerID);
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
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                Expression<Func<Lading, object>>[] includeProperties = getInclude(cols);
                var result = await _ladingRepository.GetFirstAsync(l => l.Code == ladingCode, includeProperties: includeProperties);
                if (result == null)
                {
                    return JsonError("Không tìm thấy thông tin vận đơn");
                }
                if (!(user.IsGlobalAdministrator ?? false) && result.OfficerDelivery != user.OfficerID)
                {
                    return JsonError("Bạn không được phép tra cứu vận đơn này. Vận đơn này do nhân viên khác chịu trách nhiệm");
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
            return JsonError("Không tìm thấy thông tin tài khoản");
        }
        [HttpGet("GetSingleLadingTemp")]
        public async Task<JsonResult> GetSingleLadingTemp(string ladingCode, string cols = null)
        {
            Expression<Func<LadingTemp, object>>[] includeProperties = getIncludeLadingTemp(cols);
            var result = await _ladingTempRepository.GetFirstAsync(l => l.Code == ladingCode, includeProperties: includeProperties);
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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

            if (user != null)
            {
                var result = await _ladingTempRepository.GetAsync(l => l.OfficerPickup == user.OfficerID && l.Status == (int)StatusLadingTemp.WaitingPickUp, includeProperties: includeProperties);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetLadingReport")]
        public async Task<JsonResult> GetLadingReport(int reportType)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);

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
                return JsonError("Dữ liệu đẩy vào không hợp lệ");
            }
            if (string.IsNullOrEmpty(ladingModel.Code))
            {
                return JsonError("Mã vận đơn không được bỏ trống!");
            }
            ladingModel.Code = ladingModel.Code.Trim().ToUpper();
            if (_ladingRepository.EqualsCode(ladingModel.Code))
            {
                return JsonError("Mã vận đơn đã tồn tại!");
            }
            if ((ladingModel.PriceListId ?? 0) == 0)
            {
                return JsonError("Không tìm thấy thông tin bảng giá");
            }
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {

                if (ladingModel.ExpectedTimeTransfer.HasValue && ladingModel.ExpectedTimeTransfer.Value.Subtract(DateTime.Now).TotalMinutes < 0)
                {
                    return JsonError("Ðã quá giờ trung chuyển. Vui lòng chọn chuyến khác!");
                }
                ladingModel.POFrom = user.PostOfficeId;
                ladingModel.UpdateServiceOthers();
                var computed = (await _priceRepository.Computed(ladingModel));
                if (computed.Error != 0)
                    return JsonError(computed.Message);
                var computedPrice = computed.Data;
                if (computedPrice.ChargeMain == 0)
                    return JsonError("Cước phí = 0");

                ladingModel.Amount = computedPrice.TotalCharge;
                ladingModel.DiscountAmount = computedPrice.DiscountAmount;
                ladingModel.PriceVAT = computedPrice.ChargeVAT;
                ladingModel.TotalPriceDVGT = computedPrice.ChargeAddition;
                ladingModel.PriceMain = computedPrice.ChargeMain;
                ladingModel.PPXDPercent = computedPrice.ChargeFuel;
                ladingModel.PriceOther = computedPrice.Surcharge;
                Lading lading = new Lading();
                var isDeliveryAtPO = ladingModel.RDFrom == (int)DeliveryReceiveType.VP_VP || ladingModel.RDFrom == (int)DeliveryReceiveType.DC_VP;
                var pOId = user.PostOfficeId;
                var existedCustomer = false;
                var customerExists = new Customer();
                if (isDeliveryAtPO)
                {
                    var poToObj = await _postOfficeRepository.GetFirstAsync(x => x.PostOfficeID == ladingModel.POTo && (x.IsFrom ?? false));
                    if (poToObj == null)
                    {
                        return JsonError("Không tìm thấy thông tin trạm phát");
                    }
                    ladingModel.AddressTo = poToObj.POAddress;
                    ladingModel.AddressNoteTo = "";
                    ladingModel.LatTo = poToObj.Lat;
                    ladingModel.LngTo = poToObj.Lng;
                    ladingModel.DistrictTo = poToObj.DistrictId;
                    ladingModel.CityRecipientId = poToObj.CityId;
                }
                #region Người gửi
                //if (!ladingModel.SenderId.HasValue || ladingModel.SenderId <= 0)
                //{
                customerExists = await _customerRepository.GetCustomerByPhone(ladingModel.SenderPhone);
                if (customerExists != null)
                {
                    existedCustomer = true;
                }
                //}
                //else
                //{
                //    customerExists = await _customerRepository.GetFirstAsync(o => o.CustomerID == ladingModel.SenderId.Value && o.State == 0);
                //    if (customerExists != null) existedCustomer = true;
                //}
                if (!existedCustomer)
                {
                    customerExists = new Customer
                    {
                        Address = ladingModel.SenderAddress,
                        CompanyName = ladingModel.SenderCompany,
                        Phone = ladingModel.SenderPhone,
                        CustomerName = ladingModel.SenderName,
                        State = 0,
                        AddressNote = ladingModel.AddressNoteFrom,
                        Create_Date = DateTime.Now,
                        Create_By = user.OfficerID,
                        Type = 0,//Khách vãng lai
                        PostOffice_Id = pOId,
                        Lat = ladingModel.LatFrom + "",
                        Lng = ladingModel.LatTo + "",
                        DistrictId = ladingModel.DistrictFrom,
                        CityId = ladingModel.CitySendId
                    };
                    //Insert Customer
                    _customerRepository.Insert(customerExists);
                    _customerRepository.SaveChanges();
                    customerExists.CustomerCode = _customerRepository.GetCode(customerExists.CustomerID);
                    //cus.PriceListID = await _priceListRepository.GetPriceApplyByPOID(pOId);
                    //UpdateCustomer
                    _customerRepository.SaveChanges();
                    lading.SenderId = customerExists.CustomerID;
                    ladingModel.SenderId = customerExists.CustomerID;
                }
                else
                {
                    lading.SenderId = customerExists.CustomerID;
                    ladingModel.SenderId = customerExists.CustomerID;
                    customerExists.Phone = customerExists.Phone.Trim();
                    if (string.IsNullOrEmpty(customerExists.Lng) || customerExists.Lng == "0" || (customerExists.CityId ?? 0) == 0)
                    {
                        customerExists.Lat = ladingModel.LatFrom + "";
                        customerExists.Lng = ladingModel.LatTo + "";
                        customerExists.DistrictId = ladingModel.DistrictFrom;
                        customerExists.CityId = ladingModel.CitySendId;
                    }
                    if (string.IsNullOrEmpty(customerExists.Phone2))
                        customerExists.Phone2 = !string.IsNullOrEmpty(ladingModel.PhoneFrom2) ? ladingModel.PhoneFrom2.Trim() : "";
                }

                if ((ladingModel.PaymentId ?? 0) == (int)PaymentType.Month && (customerExists.Type ?? 0) == 0)
                {
                    return JsonError("Khách lẻ không được phép thanh toán cuối tháng");
                }
                #endregion

                #region Người nhận

                var recipient = new Recipient();
                //if (ladingModel.RecipientId != null && ladingModel.RecipientId > 0)
                //{
                //    var dataR = unitOfWork.RecipientRepository.GetById(ladingModel.RecipientId.Value);
                //    recipient = dataR ?? new Recipient();
                //}
                if (ladingModel.RecipientPhone != string.Empty)
                {
                    var dataR = await _recipientRepository.GetFirstAsync(o => o.Phone == ladingModel.PhoneTo);
                    recipient = dataR ?? new Recipient();

                    recipient.State = (int)StatusSystem.Enable;
                    recipient.Name = ladingModel.RecipientName;
                    recipient.Phone = ladingModel.PhoneTo;
                    recipient.Phone2 = ladingModel.PhoneTo2;
                    recipient.CompanyName = ladingModel.RecipientCompany;
                    if (!isDeliveryAtPO || string.IsNullOrEmpty(recipient.Address)
                              || (recipient.CityId ?? 0) == 0 || (recipient.Lat ?? 0) == 0 || (recipient.Lng ?? 0) == 0)
                    {
                        recipient.Address = ladingModel.AddressTo;
                        recipient.AddressNote = ladingModel.AddressNoteTo;
                        recipient.Lat = ladingModel.LatTo;
                        recipient.Lng = ladingModel.LngTo;
                        recipient.DistrictId = ladingModel.DistrictTo;
                        recipient.CityId = ladingModel.CityRecipientId;
                    }
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
                lading.CreateDate = DateTime.Now;
                ladingModel.OficerId = user.OfficerID;
                lading.OfficerId = user.OfficerID;
                lading.OfficerPickup = user.OfficerID;
                lading.POCurrent = pOId;
                lading.ExpectedTimePickUp = DateTime.Now;
                lading.ExpectedTimeTransfer = ladingModel.ExpectedTimeTransfer ?? DateTime.Now;
                lading.ExpectedTimeDelivery = ladingModel.ExpectedTimeDelivery;
                lading.ExpectedTimeTakeOff = ladingModel.ExpectedTimeTakeOff;
                lading.POMediateId = ladingModel.POMediateId;
                lading.OrderByService = !ladingModel.OrderByService.HasValue || (ladingModel.OrderByService == 0) ? 1 : (ladingModel.OrderByService ?? 1);
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
                lading.DistanceFrom = ladingModel.DistanceFrom;
                lading.DistanceTo = ladingModel.DistanceTo;

                //lading.POFrom = ladingModel.POFrom;
                //lading.CenterFrom = ladingModel.CenterFrom;
                //lading.POTo = ladingModel.POTo;
                //lading.CenterTo = ladingModel.CenterTo;
                #region [Set PostOffice] 
                int? poFrom = user.PostOfficeId;
                int? poTo = ladingModel.POTo;

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
                    lading.CenterFrom = (await _postOfficeRepository.GetBranch(lading.POFrom ?? 0) ?? new PostOffice()).PostOfficeID;
                }
                if (_postOfficeRepository.Any(o => o.PostOfficeID == lading.POTo))
                {
                    lading.CenterTo = (await _postOfficeRepository.GetBranch(lading.POTo ?? 0) ?? new PostOffice()).PostOfficeID;
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
                lading.Width = ladingModel.Width;
                lading.Height = ladingModel.Height;
                lading.Length = ladingModel.Length;
                lading.Number = Convert.ToInt32(ladingModel.Number ?? 1);
                if (lading.Number == 0) lading.Number = 1;
                lading.Mass = Math.Round(ladingModel.Mass ?? 0, 1);
                lading.Noted = ladingModel.Noted;
                lading.Description = ladingModel.Description;
                lading.OnSiteDeliveryPrice = ladingModel.OnSiteDeliveryPrice;
                #endregion

                #region D?ch v? + Thanh toán

                lading.ServiceId = ladingModel.ServiceId;
                lading.PaymentType = ladingModel.PaymentId;
                if ((ladingModel.Amount - ladingModel.DiscountAmount) == 0) lading.PaymentType = (int)PaymentType.Done;
                if (lading.PaymentType.HasValue && lading.PaymentType == (int)PaymentType.Done)
                {
                    lading.PaymentAmount = true;
                    lading.AmountKeepBy = user.OfficerID;
                    lading.PostOfficeKeepAmount = pOId;
                }
                else
                {
                    lading.PaymentAmount = false;
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
                if (ladingModel.PPXDPercent > 0)
                {
                    lading.PPXDPercent = ladingModel.PPXDPercent;
                }
                else if (ladingModel.PPXDPrice > 0)
                {
                    lading.PPXDPercent = ladingModel.PPXDPrice;
                }
                lading.Insured = ladingModel.Insured;
                lading.InsuredPrice = Math.Round(ladingModel.InsuredPrice ?? 0);
                lading.TotalPriceDVGT = Math.Round(ladingModel.TotalPriceDVGT ?? 0);
                lading.PriceVAT = Math.Round(ladingModel.PriceVAT ?? 0);
                lading.NoteTypePack = ladingModel.NoteTypePack;
                lading.PackPrice = Math.Round(ladingModel.PackPrice ?? 0);
                lading.PackId = ladingModel.PackId;//Loại đóng gói
                lading.PriceOther = Math.Round(ladingModel.PriceOther ?? 0);
                lading.InsuredPercent = ladingModel.InsuredPercent;
                lading.KDPrice = Math.Round(ladingModel.KDPrice ?? 0);
                lading.KDNumber = ladingModel.KDNumber;
                lading.DBNDPrice = ladingModel.DBNDPrice;
                lading.DBNDFrom = ladingModel.DBNDFrom;
                lading.DBNDTo = ladingModel.DBNDTo;
                lading.DBNDNote = ladingModel.DBNDNote;
                #endregion

                #region Thông tin kiên
                if (ladingModel.Number_L_W_H_DIM_List != null && ladingModel.Number_L_W_H_DIM_List.Count > 0)
                {
                    lading.Number_L_W_H_DIM = string.Join(",", ladingModel.Number_L_W_H_DIM_List.Where(o => o.Number > 0 && o.DIM > 0).Select(o => o.Number + "_" + o.Long + "_" + o.Width + "_" + o.Height + "_" + o.DIM));
                }
                #endregion
                lading.OfficerId = user.OfficerID;
                lading.ModifiedBy = user.OfficerID;
                if ((ladingModel.PriceListId ?? 0) > 0)
                {
                    lading.PriceListId = ladingModel.PriceListId;
                }
                else
                {
                    lading.PriceListId = _priceListRepository.GetFirst(o => (o.IsApply ?? false)).PriceListID;
                }
                #region Giá + Lưu
                if (!string.IsNullOrEmpty(ladingModel.Code))
                {
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
                }
                if (lading.OfficerId != null && lading.OfficerId > 0)
                {
                    var postOffice = await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == user.PostOfficeId.Value);
                    if (postOffice != null && postOffice.PostOfficeID > 0)
                    {
                        lading.IsPriceMain = ladingModel.IsPriceMain;
                        lading.PriceMain = Math.Round(ladingModel.PriceMain ?? 0);
                        lading.Amount = Math.Round(ladingModel.Amount);
                        lading.IsConfirmByLading = ladingModel.IsMultiplePackage;
                        _ladingRepository.InsertAndInsertLadingHistory(lading);
                        if (string.IsNullOrEmpty(lading.Code))
                        {
                            lading.Code = _ladingRepository.CodeGenerationByLocationCode(postOffice.CityId ?? 0, (int)lading.Id);
                            _ladingRepository.SaveChanges();
                        }
                        if (ladingModel.AnotherServiceId != null)
                        {
                            await _ladingMapServiceRepository.AddOrEdit((int)lading.Id, computedPrice.ServiceOthers);
                        }
                        #region Discount Amount
                        if (!string.IsNullOrEmpty(ladingModel.CouponCode) && computedPrice.DiscountAmount > 0)
                        {
                            lading.DiscountAmount = ladingModel.DiscountAmount;
                            _couponRepository.Discount(ladingModel.CouponCode, user.OfficerID, (int)lading.Id, lading.DiscountAmount ?? 0);
                            _ladingRepository.SaveChanges();
                        }
                        #endregion
                        if ((lading.RDFrom == 4 || lading.RDFrom == 5) && (lading.PaymentAmount ?? false) && _customerRepository.Any(o => o.CustomerID == lading.SenderId && (o.Type ?? 0) == 0))
                        {
                            var task = ApiCustomer.LadingSendSMS((int)lading.Id, lading.POFrom ?? 0, 5);
                        }
                        if (!ladingModel.IsMultiplePackage && ladingModel.Number > 1 && (ladingModel.Number_L_W_H_DIM_List == null || ladingModel.Number_L_W_H_DIM_List.Count == 0))
                        {
                            ladingModel.Number_L_W_H_DIM_List = new List<NumberDIM>() {
                            new NumberDIM()
                            {
                                Number = Convert.ToInt32(ladingModel.Number ?? 1),
                                DIM = lading.Mass ?? 0,
                                Height = 0,
                                Long = 0,
                                Width = 0
                            }
                        };
                        }
                        if (ladingModel.Number_L_W_H_DIM_List != null && ladingModel.Number_L_W_H_DIM_List.Count > 0 && !ladingModel.IsMultiplePackage && ladingModel.Number > 1)
                        {
                            await _packageOfLadingRepository.Insert(ladingModel.Number_L_W_H_DIM_List, (int)lading.Id, user.OfficerID, pOId ?? 0);
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
                return JsonError("Dữ liệu đẩy vào không được trống");
            }
            if (!_ladingTempRepository.Any(o => o.Id == ladingModel.Id))
            {
                return JsonError("Không tìm thấy thông tin yêu cầu");
            }
            if (string.IsNullOrEmpty(ladingModel.Code))
            {
                return JsonError("Mã vận đơn không được bỏ trống");
            }
            if ((ladingModel.PriceListId ?? 0) == 0)
            {
                return JsonError("Không tìm thấy thông tin bảng giá");
            }
            ladingModel.Code = ladingModel.Code.Trim().ToUpper();
            if (_ladingRepository.EqualsCode(ladingModel.Code))
            {
                return JsonError("Mã vận đơn đã tồn tại!");
            }
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (ladingModel.ExpectedTimeTransfer.HasValue && ladingModel.ExpectedTimeTransfer.Value.Subtract(DateTime.Now).TotalMinutes < 0)
                {
                    return JsonError("Đã quá giờ trung chuyển. Vui lòng chọn chuyến khác!");
                }
                ladingModel.POFrom = user.PostOfficeId;
                ladingModel.UpdateServiceOthers();
                var computed = (await _priceRepository.Computed(ladingModel));
                if (computed.Error != 0)
                    return JsonError(computed.Message);
                var computedPrice = computed.Data;
                if (computedPrice.ChargeMain == 0)
                    return JsonError("Cước phí = 0");
                ladingModel.Amount = computedPrice.TotalCharge;
                ladingModel.DiscountAmount = computedPrice.DiscountAmount;
                ladingModel.PriceVAT = computedPrice.ChargeVAT;
                ladingModel.TotalPriceDVGT = computedPrice.ChargeAddition;
                ladingModel.PriceMain = computedPrice.ChargeMain;
                ladingModel.PPXDPercent = computedPrice.ChargeFuel;
                ladingModel.PriceOther = computedPrice.Surcharge;

                Lading lading = new Lading();
                var ladingTemp = await _ladingTempRepository.GetFirstAsync(o => o.Id == ladingModel.Id, null, o => o.Coupon);
                var postOffice = await _postOfficeRepository.GetFirstAsync(o => o.PostOfficeID == user.PostOfficeId);
                if (postOffice != null && postOffice.PostOfficeID > 0)
                {
                    if (!ladingTemp.LadingId.HasValue || !_ladingRepository.Any(o => o.Id == ladingTemp.LadingId.Value))
                    {
                        lading.Status = (int)StatusLading.DaLayHang;
                        int? poFrom = user.PostOfficeId;
                        int? poTo = ladingModel.POTo;
                        var isDeliveryAtPO = ladingModel.RDFrom == (int)DeliveryReceiveType.VP_VP || ladingModel.RDFrom == (int)DeliveryReceiveType.DC_VP;
                        if (isDeliveryAtPO)
                        {
                            var poToObj = await _postOfficeRepository.GetFirstAsync(x => x.PostOfficeID == ladingModel.POTo && (x.IsFrom ?? false));
                            if (poToObj == null)
                            {
                                return JsonError("Không tìm thấy thông tin trạm phát");
                            }
                            ladingModel.AddressTo = poToObj.POAddress;
                            ladingModel.AddressNoteTo = "";
                            ladingModel.LatTo = poToObj.Lat;
                            ladingModel.LngTo = poToObj.Lng;
                            ladingModel.DistrictTo = poToObj.DistrictId;
                            ladingModel.CityRecipientId = poToObj.CityId;
                        }
                        if ((ladingModel.LatTo ?? 0) == 0 || (ladingModel.LngTo ?? 0) == 0 || (ladingTemp.AddressTo != null && (ladingModel.AddressTo.Trim().ToUpper().Equals(ladingTemp.AddressTo.Trim().ToUpper()))))
                        {
                            ladingModel.LatTo = ladingTemp.LatTo;
                            ladingModel.LngTo = ladingTemp.LngTo;
                            ladingModel.POTo = ladingTemp.POTo;
                            poTo = ladingTemp.POTo;
                        }
                        ladingModel.SenderId = ladingTemp.SenderId;
                        var pOId = await _officeRepository.GetPOIdByOffice(user.OfficerID) ?? 0;
                        var existedCustomer = false;
                        var customerExists = new Customer();
                        #region Người gửi
                        //if (!ladingModel.SenderId.HasValue || ladingModel.SenderId <= 0)
                        //{
                        customerExists = await _customerRepository.GetCustomerByPhone(ladingModel.SenderPhone);
                        if (customerExists != null)
                        {
                            existedCustomer = true;
                        }
                        //}
                        //else
                        //{
                        //    customerExists = await _customerRepository.GetFirstAsync(o => o.CustomerID == ladingModel.SenderId.Value && o.State == 0);
                        //    if (customerExists != null) existedCustomer = true;
                        //}
                        if (!existedCustomer)
                        {
                            customerExists = new Customer
                            {
                                Address = ladingModel.SenderAddress,
                                CompanyName = ladingModel.SenderCompany,
                                Phone = ladingModel.SenderPhone,
                                CustomerName = ladingModel.SenderName,
                                State = 0,
                                AddressNote = ladingModel.AddressNoteFrom,
                                Create_Date = DateTime.Now,
                                Create_By = user.OfficerID,
                                Type = 0,//Khách vãng lai
                                PostOffice_Id = pOId,
                                Lat = ladingModel.LatFrom + "",
                                Lng = ladingModel.LngFrom + "",
                                DistrictId = ladingModel.DistrictFrom,
                                CityId = ladingModel.CitySendId
                            };
                            //Insert Customer
                            _customerRepository.Insert(customerExists);
                            _customerRepository.SaveChanges();
                            customerExists.CustomerCode = _customerRepository.GetCode(customerExists.CustomerID);
                            _customerRepository.SaveChanges();
                            lading.SenderId = customerExists.CustomerID;
                            ladingModel.SenderId = customerExists.CustomerID;
                        }
                        else
                        {
                            lading.SenderId = customerExists.CustomerID;
                            ladingModel.SenderId = customerExists.CustomerID;
                            customerExists.Phone = customerExists.Phone.Trim();
                            if (string.IsNullOrEmpty(customerExists.Lng) || customerExists.Lng == "0" || (customerExists.CityId ?? 0) == 0)
                            {
                                customerExists.Lat = ladingModel.LatFrom + "";
                                customerExists.Lng = ladingModel.LngFrom + "";
                                customerExists.DistrictId = ladingModel.DistrictFrom;
                                customerExists.CityId = ladingModel.CitySendId;
                            }
                            if (string.IsNullOrEmpty(customerExists.Phone2))
                                customerExists.Phone2 = !string.IsNullOrEmpty(ladingModel.PhoneFrom2) ? ladingModel.PhoneFrom2.Trim() : "";
                        }
                        if ((ladingModel.PaymentId ?? 0) == (int)PaymentType.Month && (customerExists.Type ?? 0) == 0)
                        {
                            return JsonError("Khách lẻ không được phép thanh toán cuối tháng");
                        }
                        #endregion

                        #region Người nhận
                        var recipient = new Recipient();
                        if (ladingModel.RecipientPhone != string.Empty)
                        {
                            var dataR = await _recipientRepository.GetFirstAsync(o => o.Phone == ladingModel.PhoneTo);
                            recipient = dataR ?? new Recipient();

                            recipient.State = (int)StatusSystem.Enable;

                            recipient.Name = ladingModel.NameTo;
                            recipient.Phone = ladingModel.PhoneTo;
                            recipient.Phone2 = ladingModel.PhoneTo2;
                            recipient.CompanyName = ladingModel.CompanyTo;
                            if (!isDeliveryAtPO || string.IsNullOrEmpty(recipient.Address)
                                || (recipient.CityId ?? 0) == 0 || (recipient.Lat ?? 0) == 0 || (recipient.Lng ?? 0) == 0)
                            {
                                recipient.Address = ladingModel.AddressTo;
                                recipient.AddressNote = ladingModel.AddressNoteTo;
                                recipient.Lat = ladingModel.LatTo;
                                recipient.Lng = ladingModel.LngTo;
                                recipient.DistrictId = ladingModel.DistrictTo;
                                recipient.CityId = ladingModel.CityRecipientId;
                            }
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
                        lading.ExpectedTimePickUp = DateTime.Now;
                        lading.ExpectedTimeTransfer = ladingModel.ExpectedTimeTransfer ?? DateTime.Now;
                        lading.ExpectedTimeDelivery = ladingModel.ExpectedTimeDelivery;
                        lading.ExpectedTimeTakeOff = ladingModel.ExpectedTimeTakeOff;
                        lading.DistanceTo = ladingModel.DistanceTo;
                        lading.DistanceFrom = ladingModel.DistanceFrom;
                        lading.POMediateId = ladingModel.POMediateId;
                        lading.OrderByService = !ladingModel.OrderByService.HasValue || (ladingModel.OrderByService == 0) ? 1 : (ladingModel.OrderByService ?? 1);
                        #region [Set PostOffice] 

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
                            lading.CenterFrom = (await _postOfficeRepository.GetBranch(lading.POFrom ?? 0) ?? new PostOffice()).PostOfficeID;
                        }
                        if (_postOfficeRepository.Any(o => o.PostOfficeID == lading.POTo))
                        {
                            lading.CenterTo = (await _postOfficeRepository.GetBranch(lading.POTo ?? 0) ?? new PostOffice()).PostOfficeID;
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
                        lading.Number = Convert.ToInt32(ladingModel.Number ?? 1);
                        if (lading.Number == 0) lading.Number = 1;
                        lading.Mass = Math.Round(ladingModel.Mass ?? 0, 1);
                        lading.Noted = ladingModel.Noted;
                        lading.Description = ladingModel.Description;
                        lading.OnSiteDeliveryPrice = ladingModel.OnSiteDeliveryPrice;
                        #endregion

                        #region Dịch vụ + Thanh toán

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
                        if (ladingModel.PPXDPercent > 0)
                        {
                            lading.PPXDPercent = ladingModel.PPXDPercent;
                        }
                        else if (ladingModel.PPXDPrice > 0)
                        {
                            lading.PPXDPercent = ladingModel.PPXDPrice;
                        }
                        lading.Insured = ladingModel.Insured;
                        lading.InsuredPrice = Math.Round(ladingModel.InsuredPrice ?? 0);
                        lading.TotalPriceDVGT = Math.Round(ladingModel.TotalPriceDVGT ?? 0);
                        lading.PriceVAT = Math.Round(ladingModel.PriceVAT ?? 0);
                        lading.NoteTypePack = ladingModel.NoteTypePack;
                        lading.PackPrice = Math.Round(ladingModel.PackPrice ?? 0);
                        lading.PackId = ladingModel.PackId;//Loại đóng gói
                        lading.PriceOther = Math.Round(ladingModel.PriceOther ?? 0);
                        lading.InsuredPercent = ladingModel.InsuredPercent;
                        lading.KDPrice = Math.Round(ladingModel.KDPrice ?? 0);
                        lading.KDNumber = ladingModel.KDNumber;
                        lading.DBNDPrice = ladingModel.DBNDPrice;
                        lading.DBNDFrom = ladingModel.DBNDFrom;
                        lading.DBNDTo = ladingModel.DBNDTo;
                        lading.DBNDNote = ladingModel.DBNDNote;
                        #endregion
                        #region Thông tin kiên
                        if (ladingModel.Number_L_W_H_DIM_List != null && ladingModel.Number_L_W_H_DIM_List.Count > 0)
                        {
                            lading.Number_L_W_H_DIM = string.Join(",", ladingModel.Number_L_W_H_DIM_List.Where(o => o.Number > 0 && o.DIM > 0).Select(o => o.Number + "_" + o.Long + "_" + o.Width + "_" + o.Height + "_" + o.DIM));
                        }
                        #endregion
                        lading.ModifiedBy = user.OfficerID;
                        if ((ladingModel.PriceListId ?? 0) > 0)
                        {
                            lading.PriceListId = ladingModel.PriceListId;
                        }

                        lading.IsPriceMain = ladingModel.IsPriceMain;
                        lading.PriceMain = Math.Round(ladingModel.PriceMain ?? 0);
                        lading.Amount = Math.Round(ladingModel.Amount);
                        #region Thêm vận đơn Lading
                        lading.Id = 0;
                        lading.OfficerId = user.OfficerID;
                        lading.OfficerPickup = user.OfficerID;
                        lading.CreateDate = DateTime.Now;
                        lading.IsConfirmByLading = ladingModel.IsMultiplePackage;
                        if ((ladingModel.Amount - ladingModel.DiscountAmount) == 0) lading.PaymentType = (int)PaymentType.Done;
                        if (lading.PaymentType.HasValue && lading.PaymentType == (int)PaymentType.Done)
                        {
                            lading.PaymentAmount = true;
                            lading.AmountKeepBy = user.OfficerID;
                            lading.PostOfficeKeepAmount = pOId;
                        }
                        else
                        {
                            lading.PaymentAmount = false;
                        }
                        _ladingRepository.InsertAndInsertLadingHistory(lading);
                        _ladingRepository.SaveChanges();
                        if (string.IsNullOrEmpty(ladingModel.Code))
                        {
                            lading.Code = _ladingRepository.CodeGenerationByLocationCode(postOffice.CityId ?? 0, (int)lading.Id);
                        }
                        else
                        {
                            lading.Code = ladingModel.Code;
                            //Cập nhập cấp vận đơn
                            var provideLading = await _provideLadingRepository.GetFirstAsync(o => o.LadingCode.ToUpper().Equals(ladingModel.Code.ToUpper()));
                            if (provideLading != null && provideLading.Status == 201)
                            {
                                provideLading.Status = 202;
                                _provideLadingRepository.Update(provideLading);
                            }
                        }
                        ladingTemp.LadingId = (int)lading.Id;
                        ladingTemp.Status = (int)StatusLadingTemp.PickedUp;
                        _ladingTempRepository.Update(ladingTemp);
                        if (ladingModel.AnotherServiceId != null)
                        {
                            await _ladingMapServiceRepository.AddOrEdit((int)lading.Id, computedPrice.ServiceOthers);
                        }
                        _ladingRepository.SaveChanges();
                        #region Discount Amount
                        var userUsingCouponId = user.OfficerID;

                        if (!string.IsNullOrEmpty(ladingModel.CouponCode) && ladingModel.DiscountAmount > 0)
                        {
                            if (ladingTemp.Coupon != null && ladingTemp.Coupon.Code.ToUpper() != ladingModel.CouponCode.ToUpper())
                            {
                                userUsingCouponId = ladingTemp.OfficerId ?? 0;
                            }
                            lading.DiscountAmount = ladingModel.DiscountAmount;
                            _couponRepository.Discount(ladingModel.CouponCode, userUsingCouponId, (int)lading.Id, lading.DiscountAmount ?? 0);
                            _ladingRepository.SaveChanges();
                        }
                        #endregion
                        if ((lading.RDFrom == 4 || lading.RDFrom == 5) && (lading.PaymentAmount ?? false) && _customerRepository.Any(o => o.CustomerID == lading.SenderId && (o.Type ?? 0) == 0))
                        {
                            var task = ApiCustomer.LadingSendSMS((int)lading.Id, lading.POFrom ?? 0, 5);
                        }
                        if (!ladingModel.IsMultiplePackage && ladingModel.Number > 1 && (ladingModel.Number_L_W_H_DIM_List == null || ladingModel.Number_L_W_H_DIM_List.Count == 0))
                        {
                            ladingModel.Number_L_W_H_DIM_List = new List<NumberDIM>() {
                            new NumberDIM()
                            {
                                Number = Convert.ToInt32(lading.Number ?? 1),
                                DIM = lading.Mass ?? 0,
                                Height = 0,
                                Long = 0,
                                Width = 0
                            }
                        };
                        }
                        if (ladingModel.Number_L_W_H_DIM_List != null && ladingModel.Number_L_W_H_DIM_List.Count > 0 && !ladingModel.IsMultiplePackage && ladingModel.Number > 1)
                        {
                            await _packageOfLadingRepository.Insert(ladingModel.Number_L_W_H_DIM_List, (int)lading.Id, user.OfficerID, pOId);
                        }
                        #endregion
                        _logger.LogInformation($"Update LadingTempID {ladingTemp.Id}: user {user.UserName}");
                        _logger.LogInformation($"Update LadingID {lading.Id}: user {user.UserName}");
                        return Json(lading);

                    }
                    else return JsonError("Yêu cầu này đã được lấy hàng thành công");
                }
                else
                {
                    return JsonError("Không tìm thấy thông tin trạm gửi");
                }
            }

            return JsonError("null");
        }
        [HttpPost("Update")]
        public async Task<JsonResult> Update([FromBody]JObject jsonData)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null && jsonData != null)
            {
                dynamic json = jsonData;
                if (jsonData["id"] != null)
                {
                    int id = json.id;
                    if (_ladingRepository.Any(l => l.Id == id))
                    {
                        Lading lading = _ladingRepository.GetSingle(l => l.Id == id);
                        var statusCurrentId = lading.Status ?? 0;
                        var note = lading.Noted;
                        var isReturn = lading.Return ?? false;
                        if (lading.State != 0)
                        {
                            return JsonError("Vận đơn đã bị hủy");
                        }
                        if (lading.CopyFromJOject(jsonData))
                        {
                            var noteUpdate = lading.Noted;
                            lading.Noted = note;
                            var statusDeliveris = new int[] { (int)StatusLading.PhatKhongTC, (int)StatusLading.HoanGoc, (int)StatusLading.ThanhCong, (int)StatusLading.DaChuyenHoan };
                            lading.ModifiedBy = user.OfficerID;
                            if ((int)StatusLading.ThanhCong == lading.Status && isReturn)
                            {
                                lading.Status = (int)StatusLading.DaChuyenHoan;
                            }
                            if ((int)StatusLading.ThanhCong == lading.Status)
                            {
                                var pointofCus = _context.CustomerPoints.Where(x => x.CustomerId == lading.SenderId).FirstOrDefault();
                                pointofCus.AllPoint += lading.Point.GetValueOrDefault();
                                pointofCus.currentpoint += lading.Point.GetValueOrDefault();
                                pointofCus.RankId = _context.Ranks.Where(x => x.MinPoint <= pointofCus.AllPoint && x.MaxPoint >= pointofCus.AllPoint && x.IsEnabled == true).FirstOrDefault().Id;
                                _context.CustomerPoints.Update(pointofCus);
                                _context.SaveChanges();
                            }
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
                            else if (statusDeliveris.Contains(lading.Status ?? 0))
                            {
                                #region  Kiện
                                var packageOfLadingIds = _context.PackageOfLadings.Where(o => (o.StatusId == (int)StatusLading.DangPhat || o.StatusId == (int)StatusLading.PhatKhongTC)
                                                        && o.DeliveryBy == user.OfficerID
                                                        && o.LadingId == lading.Id
                                                        && o.State == 0)
                                                        .Select(o => o.Id).ToArray() ?? new int[0];
                                if (packageOfLadingIds.Count() > 0)
                                {
                                    Parallel.ForEach(packageOfLadingIds, (_id) =>
                                   {
                                       using (var context = new ApplicationDbContext())
                                       {
                                           var packageOfLadingRepository = new PackageOfLadingRepository(context);
                                           var packageOfLading = packageOfLadingRepository.GetFirst(o => o.Id == _id && o.State == 0);
                                           if (packageOfLading != null)
                                           {
                                               packageOfLading.StatusId = lading.Status;
                                               if (packageOfLading.StatusId == (int)StatusLading.HoanGoc) packageOfLading.Return = true;
                                               if (packageOfLading.StatusId == (int)StatusLading.ThanhCong ||
                                                   packageOfLading.StatusId == (int)StatusLading.DaChuyenHoan)
                                               {
                                                   packageOfLading.RecipientReality = lading.Recipient_reality;
                                                   packageOfLading.FinishDate = DateTime.Now;
                                               }
                                               packageOfLadingRepository.Update(user.OfficerID, user.PostOfficeId ?? 0, packageOfLading).Wait();
                                           }
                                       }
                                   });
                                    await _packageOfLadingRepository.UpdateIsPartStatusLading((int)id);
                                }
                                #endregion
                            }

                            if (statusCurrentId == lading.Status && lading.Status != (int)StatusLading.PhatKhongTC)
                            {
                                _ladingRepository.Update(lading);
                                _ladingRepository.SaveChanges();
                            }
                            else
                            {
                                string location = (string)json.location;
                                int? typeReasonID = (int?)json.typeReasonID;
                                if (lading.Status == (int)StatusLading.NVKhongNhan || lading.Status == (int)StatusLading.LayHangKhongTC ||
                                    lading.Status == (int)StatusLading.PhatKhongTC)
                                {
                                    _ladingRepository.UpdateAndInsertLadingHistory(lading, user, typeReasonID, location, noteUpdate);
                                }
                                else
                                {
                                    _ladingRepository.UpdateAndInsertLadingHistory(lading, user, null, location, noteUpdate);
                                }
                                if (_customerRepository.Any(x => x.CustomerID == lading.SenderId && x.PartnerId == (int)EMSHelper.PARTNER_ID) && statusDeliveris.Contains(lading.Status ?? 0))
                                {
                                    var emsNote = "";
                                    var reason = "";
                                    if (lading.Status == (int)StatusLading.PhatKhongTC && typeReasonID.HasValue)
                                    {
                                        emsNote = ((lading.TimesDelivery ?? 0) > 1 ? ". Phát không thành công lần " + lading.TimesDelivery + ". " : "");
                                        var typeReason = _context.TypeReasons.FirstOrDefault(x => x.TypeReasonID == typeReasonID);
                                        if (typeReason != null)
                                        {
                                            reason = typeReason.Code;
                                            emsNote += ((typeReason.IsText) ? noteUpdate : typeReason.TypeReasonName);
                                        }
                                    }
                                    int cityId = ((lading.Return ?? false) ? lading.CitySendId : lading.CityRecipientId) ?? 0;
                                    var city = _locationRepository.GetFirst(x => x.LocationId == cityId);
                                    await _iEMSService.Delivery(new EMSLadingDeliveryRequest(lading.Code, EMSHelper.ConvertToEMSStatus(lading.Status ?? 0, lading.Return ?? false),
                                       city?.LocationEMSId + "", city?.PostOffficeEMSCode, DateTime.Now, emsNote, reason));
                                    if (lading.Status == (int)StatusLading.ThanhCong && !string.IsNullOrEmpty(lading.ImageDelivery))
                                    {
                                        var path = System.IO.Path.Combine(_env.ContentRootPath + lading.ImageDelivery);
                                        if (System.IO.File.Exists(path))
                                            await _iEMSService.DeliveryImage(new EMSLadingDeliveryImageRequest(lading.Code, ImageHelper.ImageToBase64(System.Drawing.Image.FromFile(path), System.Drawing.Imaging.ImageFormat.Jpeg)));
                                    }
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
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
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
                            lading.Status = Helper.Helper.GetStatusLadingTemp(lading.Status ?? 0);
                            if ((int)StatusLadingTemp.WaitingPickUp == lading.Status)
                            {
                                lading.OfficerPickup = user.OfficerID;
                            }
                            else if (lading.Status == (int)StatusLadingTemp.RefusePickUp || lading.Status == (int)StatusLadingTemp.PickedFail)
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
        #region EMS
        [AllowAnonymous]
        [HttpPost("EMSDelivery")]
        public async Task<JsonResult> EMSDelivery([FromBody]EMSLadingDeliveryRequest request)
        {
            return JsonSuccess(await _iEMSService.Delivery(request));
        }
        [AllowAnonymous]
        [HttpGet("EMSGetLading")]
        public async Task<JsonResult> EMSGetLading(string code)
        {
            return JsonSuccess(await _iEMSService.GetLading(code));
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            _ladingRepository.Dispose();
            base.Dispose(disposing);
        }
        #region GET INCLUDES
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
                    else if (colValue == "pricelistid")
                    {
                        includeProperties.Add(inc => inc.PriceList);
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
                    else if (colValue == "pricelistid")
                    {
                        includeProperties.Add(inc => inc.PriceList);
                    }
                }
            }
            return includeProperties.ToArray();
        }
        #endregion
    }
}