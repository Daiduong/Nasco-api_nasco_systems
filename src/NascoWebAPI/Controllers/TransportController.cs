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

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Transport")]
    [Authorize]
    public class TransportController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ITransportRepository _transportRepository;
        public TransportController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ITransportRepository TransportRepository
            ) : base(logger)
        {
            _transportRepository = TransportRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [AllowAnonymous]
        [HttpGet("GetSingle")]
        public async Task<JsonResult> GetSingle( int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _transportRepository.GetSingleAsync(s => s.Id == id);
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [AllowAnonymous]
        [HttpGet("GetListByGlobal")]

        public async Task<JsonResult> GetListByGlobal(bool isGlobal = false)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _transportRepository.GetAsync(s => s.IsGlobal == isGlobal);
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [AllowAnonymous]
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetList()
        {
            var result = await _transportRepository.GetAllAsync();
            return Json(result);
        }
        #endregion
    }
}