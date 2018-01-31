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
    [Route("api/DeliveryReceive")]
    [AllowAnonymous]
    public class DeliveryReceiveController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IDeliveryReceiveRepository _DeliveryReceiveRepository;
        public DeliveryReceiveController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IDeliveryReceiveRepository DeliveryReceiveRepository
            ) : base(logger)
        {
            _DeliveryReceiveRepository = DeliveryReceiveRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetAll()
        {
            var result = await _DeliveryReceiveRepository.GetAsync( o => (o.ShowRequest ?? false), or => or.OrderBy(o=> o.Index));
            return Json(result);

        }
        #endregion
    }
}