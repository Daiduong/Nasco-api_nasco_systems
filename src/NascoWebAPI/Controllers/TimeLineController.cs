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
using NascoWebAPI.Helper.Common;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/TimeLine")]
    [Authorize]
    public class TimeLineController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ITimeLineRepository _timeLineRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        public TimeLineController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IPostOfficeRepository postOfficeRepository,
            ITimeLineRepository timeLineRepository
            ) : base(logger)
        {
            _timeLineRepository = timeLineRepository;
            _officeRepository = officeRepository;
            _postOfficeRepository = postOfficeRepository;
        }
        #region [Get]
        [HttpGet("GetListExpectedTime")]
        public async Task<JsonResult> GetListExpectedTime(int cityFromId, int cityToId, int serviceId, int? poTo = null, int? deliveryReceiveId = null)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                int? poToLevel = null;
                if (poTo.HasValue) poToLevel = (await _postOfficeRepository.GetSingleAsync(x => x.PostOfficeID == poTo && x.State == 0))?.Level;
                var result = await _timeLineRepository.GetListExpectedTime(cityFromId, cityToId, serviceId, user.PostOfficeId ?? 0, poToLevel, deliveryReceiveId);
                return Json(result);
            }
            return JsonError("null");
        }
        #endregion
    }
}