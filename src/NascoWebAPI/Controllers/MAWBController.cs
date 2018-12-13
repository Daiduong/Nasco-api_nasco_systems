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
using System.Linq.Expressions;
using NascoWebAPI.Helper;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/MAWB")]
    [Authorize]
    public class MAWBController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IMAWBRepository _mAWBRepository;
        private readonly IAirlineRepository _airlineRepository;
        public MAWBController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IMAWBRepository MAWBRepository,
             IAirlineRepository airlineRepository
            ) : base(logger)
        {
            _mAWBRepository = MAWBRepository;
            _officeRepository = officeRepository;
            _airlineRepository = airlineRepository;
        }
        #region Quản lý  lịch bay
        [HttpGet("GetListMAWBNew")]
        public async Task<JsonResult> GetListMAWBNew()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    return JsonSuccess(await _mAWBRepository.GetListMAWBNew(user.PostOfficeId ?? 0));
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        [HttpPost("AddOrEdit")]
        public async Task<JsonResult> AddOrEdit([FromBody]MAWBModel model)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    model.ModifiedBy = user.OfficerID;
                    var result = await _mAWBRepository.AddOrEdit(model);
                    return result.Error == 0 ? JsonSuccess(result.Data) : JsonError(result.Message);
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        #endregion
        [HttpGet("GetListAirline")]
        public async Task<JsonResult> GetListAirline()
        {
            return JsonSuccess(await _airlineRepository.GetAsync());
        }
        [HttpGet("GetList")]
        public async Task<JsonResult> GetList(int? pageSize, int? pageNo, DateTime? dateFrom, DateTime? dateTo, int? poToId, string code)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    int poCurrentId = user.PostOfficeId ?? 0;
                    Expression<Func<MAWB, bool>> predicate = l => l.PoFrom == poCurrentId;
                    if (dateFrom.HasValue)
                    {
                        predicate = predicate.And(x => dateFrom.Value.Date <= x.ExpectedDateTime.Value.Date);
                    }
                    if (dateTo.HasValue)
                    {
                        predicate = predicate.And(x => x.ExpectedDateTime.Value.Date <= dateTo.Value.Date);
                    }
                    if (poToId.HasValue)
                    {
                        predicate = predicate.And(x => x.PoTo == poToId);
                    }
                    if (!string.IsNullOrEmpty(code))
                    {
                        code = code.ToUpper();
                        predicate = predicate.And(x => x.MAWBCode != null && x.MAWBCode.ToUpper().Contains(code));
                    }
                    int? take = null;
                    int? skip = null;
                    if ((pageSize ?? 0) >= 0)
                        take = pageSize.Value;
                    if ((pageSize ?? 0) >= 0 && (pageNo ?? 0) >= 1)
                        skip = ((pageNo ?? 0) - 1) * (pageSize ?? 0);

                    return JsonSuccess(await _mAWBRepository.GetAsync(predicate, null, skip, take, x=> x.Airline, x => x.PoFromObj, x => x.PoToObj));
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
        [HttpGet("GetById")]
        public async Task<JsonResult> GetById(int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetFirstAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (user.JobId == (int)JobType.KTBAY)
                {
                    return JsonSuccess(await _mAWBRepository.GetSingleAsync(x => x.Id == id));
                }
                return JsonError("Nhân viên phải thuộc bộ phận KHAI THÁC BAY");
            }
            return JsonError("Không tìm thấy thông tin người dùng");
        }
    }
}