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
using NascoWebAPI.Models;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Service")]
    [Authorize]
    public class ServiceController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ILocationRepository _locationRepository;
        private readonly IPostOfficeRepository _postOfficeRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IGroupServiceRepository _groupServiceRepository;

        public ServiceController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IServiceRepository serviceRepository,
            ILocationRepository locationRepository,
             IPostOfficeRepository postOfficeRepository,
             IGroupServiceRepository groupServiceRepository
            ) : base(logger)
        {
            _serviceRepository = serviceRepository;
            _officeRepository = officeRepository;
            _locationRepository = locationRepository;
            _postOfficeRepository = postOfficeRepository;
            _groupServiceRepository = groupServiceRepository;
        }
        #region [Get]
        [HttpGet("GetSingle")]
        [AllowAnonymous]
        public async Task<JsonResult> GetSingle(int id)
        {
            var result = await _serviceRepository.GetSingleAsync(s => s.ServiceID == id);
            if (result == null)
            {
                return JsonError("Not Found");
            }
            return Json(result);
        }
        [HttpGet("GetListMainService")]
        [AllowAnonymous]
        public async Task<JsonResult> GetListMainService()
        {

            var result = await _serviceRepository.GetListMainService();
            return Json(result);

        }
        [HttpGet("GetListSupportService")] 
        [AllowAnonymous]
        public async Task<JsonResult> GetListSupportService()
        {
            var result = await _serviceRepository.GetListSupportService();
            return Json(result);
        }

        [HttpGet("GetListSupportServiceByTransport")]
        [AllowAnonymous]
        public async Task<JsonResult> GetListSupportServiceByTransport(int transportId)
        {
            var result = await _serviceRepository.GetListSupportServiceByTransport(transportId);
            return Json(result);
        }
        [HttpPost("GetListMainServiceByLading")]
        public JsonResult GetListMainServiceByLading([FromBody]LadingViewModel ladingModel)
        {
            var services = _serviceRepository.GetListMainServiceByLading(ladingModel);
            var groupServices = Enumerable.Empty<GroupService>();
            if (services.Count() > 0)
            {
                var groupServiceIds = services.Where(o => o.GSId.HasValue).Select(o => o.GSId.Value).ToList();
                if (groupServiceIds.Count() > 0)
                {
                    groupServices = _groupServiceRepository.Get(o => groupServiceIds.Contains(o.Id));
                }
            }

            return Json(new { Services = services, GroupServices = groupServices });
        }
        #endregion
    }
}