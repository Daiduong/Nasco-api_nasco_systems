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
    [Route("api/AutoTask")]
    [Authorize]
    public class AutoTaskController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly AutoServices _autoServices;
        public AutoTaskController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IServiceRepository serviceRepository,
            ApplicationDbContext context
            ) : base(logger)
        {
            _serviceRepository = serviceRepository;
            _officeRepository = officeRepository;
            _autoServices = new AutoServices(context);
        }
        #region [Get]
        [HttpGet("GetSingle")]
        public async Task<JsonResult> GetSingle(int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _serviceRepository.GetSingleAsync(s => s.ServiceID == id);
                if (result == null)
                {
                    return JsonError("BBB");
                }
                return Json(result);
            }
            return JsonError("OHT");
        }

        [HttpGet("GetPickUpAuto")]
        public async Task<JsonResult> GetPickUpAuto(int poid)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _autoServices.GetPickUpAuto(poid);
                return JsonError("Success.");
            }
            else
            {
                return JsonError("Login failed!");
            }
        }
        [HttpGet("RePickupLadingAuto")]
        public async Task<JsonResult> RePickupLadingAuto(int poid)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _autoServices.RePickupLadingAuto(poid);
                return JsonError("Success.");
            }
            else
            {
                return JsonError("Login failed!");
            }
        }
        #endregion
    }
}