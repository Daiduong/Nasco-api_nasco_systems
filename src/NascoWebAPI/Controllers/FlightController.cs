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
using NascoWebAPI.Helper.Common;
using System.Linq.Expressions;
using NascoWebAPI.Helper;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Flight")]
    [Authorize]
    public class FlightController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IFlightRepository _flightRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        private readonly IBKInternalRepository _bKInternalRepository;
        private readonly IMAWBRepository _mAWBRepository;
        public FlightController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IFlightRepository flightRepository,
            IPostOfficeRepository postOfficeRepository,
             IBKInternalRepository bKInternalRepository,
             IMAWBRepository mAWBRepository
            ) : base(logger)
        {
            _flightRepository = flightRepository;
            _officeRepository = officeRepository;
            _postOfficeRepository = postOfficeRepository;
            _bKInternalRepository = bKInternalRepository;
            _mAWBRepository = mAWBRepository;
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
                    return result.Error == 0 ? JsonSuccess(result.Data) : JsonError(result.Message);
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
                    return result.Error == 0 ? JsonSuccess(result.Data) : JsonError(result.Message);
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        #endregion
        #region Tạo hoặc điều chỉnh chuyến bay 
        [HttpPost("AddOrEdit")]
        public async Task<JsonResult> AddOrEdit([FromBody]FlightModel model)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    model.ModifiedBy = user.OfficerID;
                    var result = await _flightRepository.AddOrEdit(model);
                    return result.Error == 0 ? JsonSuccess(result.Data) : JsonError(result.Message);
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        #endregion
        #region Danh sách bảng kê đi bay
        [HttpGet("GetListBKInternalWaitingToFly")]
        public async Task<JsonResult> GetListBKInternalWaitingToFly(int? mawbId)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    var poId = (await _mAWBRepository.GetSingleAsync(x => x.Id == mawbId))?.PoTo;
                    if (!poId.HasValue)
                    {
                        return JsonError("Vui lòng chọn AWB");
                    }
                    int poCurrentId = user.PostOfficeId ?? 0;
                    var po = await _postOfficeRepository.GetSingleAsync(x => x.PostOfficeID == poCurrentId);
                    if (po == null)
                    {
                        return JsonError("Không tìm thấy bưu cục mà người dùng trực thuộc");
                    }
                    var bks = Enumerable.Empty<BKInternal>();
                    if ((po.PostOfficeTypeId ?? 0) == (int)PostOfficeType.HUB)
                    {
                        var poIds = (await _postOfficeRepository.GetListFromCenter(poCurrentId)).Select(o => o.PostOfficeID).ToList();
                        bks = await _bKInternalRepository.GetAsync(o => poIds.Contains(o.POCreate.Value) && o.POCreate.HasValue && o.IsFly.Value && o.Status.Value == (int)StatusFlight.Confirmed, null, null, null, x => x.PostOfficeTo);
                    }
                    if (poId.HasValue)
                    {
                        var poIds = (await _postOfficeRepository.GetListFromCenter(poId.Value)).Select(o => o.PostOfficeID).ToList();
                        bks = bks.Where(o => poIds.Contains(o.PostOfficeId.Value));
                    }
                    return JsonSuccess(bks);
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        #endregion
        #region Danh sách bảng kê theo chuyến bay
        [HttpGet("GetListBKInteranlByFlight")]
        public async Task<JsonResult> GetListBKInteranlByFlight(int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    return JsonSuccess(await _bKInternalRepository.GetAsync(o => o.FlightId == id, null, null, null, x => x.PostOfficeTo));
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        #endregion
        [HttpGet("GetById")]
        public async Task<JsonResult> GetById(int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    return JsonSuccess(await _flightRepository.GetSingleAsync(o => o.Id == id && o.State == 0, x => x.PostOfficeTo, x => x.MAWB));
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        [HttpGet("GetList")]
        public async Task<JsonResult> GetList(int? pageSize, int? pageNo, DateTime? dateFrom, DateTime? dateTo, int? poToId)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    int poCurrentId = user.PostOfficeId ?? 0;
                    Expression<Func<Flight, bool>> predicate = l => l.POFrom == poCurrentId && l.State == 0;
                    if (dateFrom.HasValue)
                    {
                        predicate = predicate.And(s => dateFrom.Value.Date <= s.Date_Create.Value.Date);
                    }
                    if (dateTo.HasValue)
                    {
                        predicate = predicate.And(s => s.Date_Create.Value.Date <= dateTo.Value.Date);
                    }
                    if (poToId.HasValue)
                    {
                        predicate = predicate.And(x => x.POTo == poToId);
                    }
                    int? take = null;
                    int? skip = null;
                    if ((pageSize ?? 0) >= 0)
                        take = pageSize.Value;
                    if ((pageSize ?? 0) >= 0 && (pageNo ?? 0) >= 1)
                        skip = ((pageNo ?? 0) - 1) * (pageSize ?? 0);

                    return JsonSuccess(await _flightRepository.GetAsync(predicate, null, skip, take, x => x.PostOfficeTo, x => x.MAWB));
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
    }
}