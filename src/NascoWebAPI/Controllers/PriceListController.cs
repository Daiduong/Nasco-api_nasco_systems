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
    [Route("api/PriceList")]
    public class PriceListController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IPriceListRepository _priceListRepository;
        public PriceListController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IPriceListRepository priceListRepository
            ) : base(logger)
        {
            _priceListRepository = priceListRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [AllowAnonymous]
        [HttpGet("GetListPriceListByCustomer")]
        public JsonResult GetListPriceListByCustomer(int? id = null, bool union = true)
        {
            return Json(_priceListRepository.GetListPriceListByCustomer(id, union).OrderByDescending(o => o.IsApply).ThenBy(o => o.PriceListTypeId));
        }
        #endregion
    }
}