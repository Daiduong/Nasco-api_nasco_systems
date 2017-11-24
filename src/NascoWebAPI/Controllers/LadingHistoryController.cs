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
using System.Linq.Expressions;
using NascoWebAPI.Helper;
using NascoWebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/LadingHistory")]
    [Authorize]
    public class LadingHistoryController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ILadingHistoryRepository _LadingHistoryRepository;
        public LadingHistoryController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ILadingHistoryRepository LadingHistoryRepository
            ) : base(logger)
        {
            _LadingHistoryRepository = LadingHistoryRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetByLading")]
        public async Task<JsonResult> GetByLading(int ladingId, bool currentUser = false, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<LadingHistory, object>>[] includeProperties = getInclude(cols);
                Expression<Func<LadingHistory, bool>> predicate = s => s.LadingId == ladingId;
                if (currentUser)
                {
                    predicate = s => s.LadingId == ladingId && s.OfficerId == user.OfficerID;
                }
                var result = await _LadingHistoryRepository.GetAsync(predicate, orderBy: s => s.OrderByDescending(l => l.Id), includeProperties: includeProperties);
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetByCurrentUser")]
        public async Task<JsonResult> GetByCurrentUser(DateTime? dateFrom, DateTime? dateTo, int? pageCurrent = null, int? pageSize = null, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<LadingHistory, object>>[] includeProperties = getInclude(cols);
                //Expression<Func<LadingHistory, bool>> predicate = PredicateBuilder.And(l => l.OfficerId == user.OfficerID, getFilter(dateFrom, dateTo));

                var result = Enumerable.Empty<LadingHistory>();

                result = await _LadingHistoryRepository.GetByUser(user.OfficerID, dateFrom, dateTo,
                    includeProperties: includeProperties,
                    take: pageSize, skip: pageSize * (pageCurrent - 1));

                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetByOwn")]
        public async Task<JsonResult> GetByOwn(DateTime? dateFrom, DateTime? dateTo, int? pageCurrent = null, int? pageSize = null, string cols = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                Expression<Func<LadingHistory, object>>[] includeProperties = getInclude(cols);
                Expression<Func<LadingHistory, bool>> predicate = getFilter(dateFrom, dateTo);
                var result = await _LadingHistoryRepository.GetDistinctLading(dateFrom, dateTo,
                    includeProperties: includeProperties,
                    take: pageSize, skip: pageSize * (pageCurrent - 1));
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }

        private Expression<Func<LadingHistory, bool>> getFilter(DateTime? dateFrom, DateTime? dateTo)
        {
            if (!dateFrom.HasValue && !dateTo.HasValue)
            {
                dateFrom = dateTo = DateTime.Now;
            }
            if (dateFrom.HasValue && dateTo.HasValue)
            {
                return r => dateFrom.Value.Date <= r.DateTime.Value.Date && r.DateTime.Value.Date <= dateTo.Value.Date;
            }
            else if (dateFrom.HasValue)
            {
                return r => dateFrom.Value.Date <= r.DateTime.Value.Date;
            }
            else
            {
                return r => r.DateTime.Value.Date <= dateTo.Value.Date;
            }
        }
        #endregion

        private Expression<Func<LadingHistory, object>>[] getInclude(string cols)
        {
            List<Expression<Func<LadingHistory, object>>> includeProperties = new List<Expression<Func<LadingHistory, object>>>();
            if (!string.IsNullOrEmpty(cols))
            {
                foreach (var col in cols.Split(','))
                {
                    var colValue = col.Trim().ToLower();
                    if (colValue == "status")
                    {
                        includeProperties.Add(inc => inc.CurrenSttStatus);
                    }
                    else if (colValue == "postofficeid")
                    {
                        includeProperties.Add(inc => inc.PostOffice);
                    }
                    else if (colValue == "typereason")
                    {
                        includeProperties.Add(inc => inc.CurrentTypeReason);
                    }
                    else if (colValue == "officeid")
                    {
                        includeProperties.Add(inc => inc.Officer);
                    }
                    else if (colValue == "ladingid")
                    {
                        includeProperties.Add(inc => inc.Lading);
                    }
                }
            }
            return includeProperties.ToArray();
        }
    }
}