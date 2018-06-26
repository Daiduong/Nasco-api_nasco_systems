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
    [Route("api/Price")]
    public class PriceController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IPriceRepository _PriceRepository;
        private readonly IServiceRepository _serviceRepository;
        private readonly IGroupServiceRepository _groupServiceRepository;
        public PriceController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
                        IServiceRepository serviceRepository,
                        IGroupServiceRepository groupServiceRepository,
            IPriceRepository PriceRepository
            ) : base(logger)
        {
            _PriceRepository = PriceRepository;
            _officeRepository = officeRepository;
            _groupServiceRepository = groupServiceRepository;
            _serviceRepository = serviceRepository;
        }
        #region [Get]
        [AllowAnonymous]
        [HttpPost("CalculatePrice")]
        public async Task<JsonResult> CalculatePrice([FromBody]LadingViewModel ladingModel)
        {
            if (ladingModel == null)
            {
                return JsonError("Not Empty");
            }
            var result = await _PriceRepository.Computed(ladingModel);
            return Json(new CalculatePriceModel(result.Data)
            {
                Message = result.Message
            });
        }
        [AllowAnonymous]
        [HttpPost("GetListPrice")]
        public async Task<JsonResult> GetListPrice([FromBody]LadingViewModel ladingModel)
        {
            if (ladingModel == null)
            {
                return JsonError("Not Empty");
            }
            double weightD = (ladingModel.Weight ?? 0) < (ladingModel.Mass ?? 0) ? (ladingModel.Mass ?? 0) : (ladingModel.Weight ?? 0);
            var result = _PriceRepository.GetListPrice(weightD, ladingModel.SenderId ?? 0, ladingModel.CitySendId ?? 0, ladingModel.CityRecipientId ?? 0, ladingModel.RDFrom ?? 0);
            if (result.Count() > 0)
            {
                result.ToList().ForEach(o =>
                {

                });
            }
            return Json(result);
        }
        #endregion
    }
}