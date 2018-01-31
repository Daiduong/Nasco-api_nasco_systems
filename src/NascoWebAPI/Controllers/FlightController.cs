using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using NascoWebAPI.Data;
using NascoWebAPI.Helper.JwtBearerAuthentication;
using Microsoft.Extensions.Logging;
using static NascoWebAPI.Helper.Common.Constants;
using NascoWebAPI.Models;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Flight")]
    [Authorize]
    public class FlightController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IFlightRepository _flightRepository;
        public FlightController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IFlightRepository flightRepository
            ) : base(logger)
        {
            _flightRepository = flightRepository;
            _officeRepository = officeRepository;
        }
        [HttpGet("GetListFlightWaitingForTakeOff")]
        public async Task<JsonResult> GetListFlightWaitingForTakeOff(int? poTo, int? pageSize = null, int? pageNo = null, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var statuses = new int[] { (int)StatusFlight.Created };
                return Json(await _flightRepository.GetListFlight(user.PostOfficeId, poTo, statuses, pageSize, pageNo, cols));
            }
            return JsonError("Không tìm thấy thông tin user");
        }
        [HttpGet("GetListFlightWaitingForReceive")]
        public async Task<JsonResult> GetListFlightWaitingForReceive(int? poFrom, int? pageSize = null, int? pageNo = null, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var statuses = new int[] { (int)StatusFlight.TakeOff };
                return Json(await _flightRepository.GetListFlight(poFrom, user.PostOfficeId, statuses, pageSize, pageNo, cols));
            }
            return JsonError("Không tìm thấy thông tin user");
        }
        #region Rời sân bay 
        [HttpPost("TakeOff")]
        public async Task<JsonResult> TakeOff([FromBody]FlightModel model)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    model.ModifiedBy = user.OfficerID;
                    var result = await _flightRepository.TakeOff(model);
                    return result.Error == 0 ? Json(result.Data) : JsonError(result.Message);
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin user");
        }
        #endregion
        #region Nhận hàng bay 
        [HttpPost("Receive")]
        public async Task<JsonResult> Receive([FromBody]FlightModel model)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    model.ModifiedBy = user.OfficerID;
                    var result = await _flightRepository.Receive(model);
                    return result.Error == 0 ? Json(result.Data) : JsonError(result.Message);
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin user");
        }
        #endregion
    }
}