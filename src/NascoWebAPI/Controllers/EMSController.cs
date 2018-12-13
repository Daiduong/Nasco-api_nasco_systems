using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Data;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;
using NascoWebAPI.Models;
using NascoWebAPI.Services;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/EMS")]
    [Authorize]
    public class EMSController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly ICustomerContactRepository _customerContactRepository;
        private readonly ILadingRepository _ladingRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IEMSService _iEMSService;
        private readonly ApplicationDbContext _context;
        public EMSController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ICustomerRepository customerRepository,
            ILadingRepository ladingRepository,
            IPostOfficeRepository postOfficeRepository,
            ILocationRepository locationRepository,
            IEMSService iEMSService,
            ICustomerContactRepository customerContactRepository,
            ApplicationDbContext context
            ) : base(logger)
        {
            _customerRepository = customerRepository;
            _officeRepository = officeRepository;
            _ladingRepository = ladingRepository;
            _postOfficeRepository = postOfficeRepository;
            _iEMSService = iEMSService;
            _locationRepository = locationRepository;
            _context = context;
            _customerContactRepository = customerContactRepository;
        }
        [HttpGet("GetListEMSPO")]
        public async Task<JsonResult> GetListEMSPO()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject, x => x.PostOffice);
            if (user == null)
                return JsonError("Không tìm thấy thông tin người dùng");
            var cityId = user.PostOffice?.CityId;
            return JsonSuccess((await _customerContactRepository.GetListContactByPartnerId(EMSHelper.PARTNER_ID).Where(x => x.CityId == cityId).ToListAsync()).Select(x => EntityHelper.ConvertToCustomer(x)));
        }
        [AllowAnonymous]
        [HttpPost("InsertLading")]
        public async Task<JsonResult> InsertLading([FromBody] EMSLadingModel model)
        {
            if (model == null)
            {
                return JsonError("Dữ liệu không hợp lệ");
            }
            Officer user = null;
            var statusId = (int)StatusLading.DaLayHang;
            if (Constants.KEY_API_SYSTEM.Equals(Request.Headers["Key"] + ""))
            {
                user = await _officeRepository.GetFirstAsync(o => o.OfficerID == model.UserId);
                if (model.StatusId != 0) statusId = model.StatusId;
            }
            else if (!string.IsNullOrEmpty(Request.Headers["Authorization"]))
            {
                var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
                user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            }
            if (user == null)
                return JsonError("Không tìm thấy thông tin người dùng");
            var result = await _ladingRepository.InsertEMS(user.OfficerID, model.CustomerId, model.ServiceId, model.Code, statusId);
            return result.Error == 0 ? JsonSuccess(result.Data) : JsonError(result.Message);
        }
        [AllowAnonymous]
        [HttpGet("UpdateLading")]
        public async Task<JsonResult> UpdateLading()
        {
            if (Constants.KEY_API_SYSTEM.Equals(Request.Headers["Key"] + ""))
            {
                var result = await _ladingRepository.UpdateEMS();
                return JsonSuccess(message: result.Message);
            }
            return JsonError("Not Found");
        }
        [AllowAnonymous]
        [HttpPost("UpdateLadingStatus")]
        public async Task<JsonResult> UpdateLadingStatus([FromBody]EMSLadingStatusModel model)
        {
            if (model == null)
            {
                _logger.LogError($"model null");
                return JsonError("Dữ liệu đầu vào không hợp lệ");
            }
            var result = await _ladingRepository.UpdateToPartner(EMSHelper.PARTNER_ID, model.LadingId, model.POCurrentId, model.StatusId, model.DateTime);
            if (result.Error == 1)
            {
                _logger.LogError(result.Message);
                return JsonError(result.Message);
            };
            return JsonSuccess(result);
        }
        public class EMSLadingModel
        {
            public string Code { get; set; }
            public int CustomerId { get; set; }
            public int ServiceId { get; set; }
            public int UserId { get; set; }
            public int StatusId { get; set; }

        }
        public class EMSLadingStatusModel
        {
            public int LadingId { get; set; }
            public int POCurrentId { get; set; }
            public int StatusId { get; set; }
            public DateTime? DateTime { get; set; }
        }
    }
}