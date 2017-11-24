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

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/BKInternal")]
    [Authorize]
    public class BKInternalController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IBKInternalRepository _bkInternalRepository;
        private readonly ILadingRepository _ladingRepository;

        public BKInternalController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IBKInternalRepository BKInternalRepository,
            ILadingRepository ladingRepository
            ) : base(logger)
        {
            _bkInternalRepository = BKInternalRepository;
            _officeRepository = officeRepository;
            _ladingRepository = ladingRepository;
        }
        #region [Get]
        [HttpGet("GetListWaiting")]
        public async Task<JsonResult> GetListWaiting()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _bkInternalRepository.GetListWaitingByOfficer(user.OfficerID);
                return Json(result);
            }
            return JsonError("Kh�ng t�m th?y th�ng tin ng??i d�ng");
        }
        [HttpGet("GetListLading")]
        public async Task<JsonResult> GetListLading(int id, string cols)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                if (!_bkInternalRepository.Any(o => o.ID_BK_internal == id))
                {
                    return JsonError("Kh�ng t�m th?y b?ng k�");
                }
                var bkInternal = await _bkInternalRepository.GetSingleAsync(o => o.ID_BK_internal == id);
                var listLadingID = bkInternal.ListLadingId.Replace("//", ";").Split(';');
                return Json(await _ladingRepository.GetAsync(l => listLadingID.Contains(l.Id.ToString()), includeProperties: IncludeUtil.GetLadingInclude(cols)));
            }
            return JsonError("Kh�ng t�m th?y th�ng tin ng??i d�ng");
        }
        #endregion

    }
}