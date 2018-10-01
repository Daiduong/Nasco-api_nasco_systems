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
    [Route("api/PostOffice")]
    [Authorize]
    public class PostOfficeController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        public PostOfficeController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IPostOfficeRepository postOfficeRepository
            ) : base(logger)
        {
            _postOfficeRepository = postOfficeRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetListPostOfficeTo")]
        public async Task<JsonResult> GetListPostOfficeTo()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _postOfficeRepository.GetListFromRoot((int)PostOfficeMethod.TO);
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetListPostOfficeCenterTo")]
        public async Task<JsonResult> GetListPostOfficeCenterTo()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _postOfficeRepository.GetAsync(p => p.PostOfficeTypeId == (int)PostOfficeType.HUB && p.State == 0);
                return Json(result);
            }
            return JsonError("null");
        }
        #endregion
    }
}