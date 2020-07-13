using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Data;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/TPL")]
    [Authorize]
    public class TPLController : BaseController
    {
        private readonly ITPLEMSRepository _tplemsRepository;
        public TPLController(ILogger<UserController> logger,
           ITPLEMSRepository TPLEMSRepository
           ) : base(logger)
        {
            _tplemsRepository = TPLEMSRepository;
        }

        [AllowAnonymous]
        [HttpGet("GetInventory")]
        public async Task<JsonResult> GetInventory()
        {
            var json =await _tplemsRepository.GetInventory();
            return JsonSuccess(json);
        }
        [  AllowAnonymous]
        [HttpGet("InsertInventory")]
        public async Task<JsonResult> InsertInventory(string name, string username,
                        string phone, string provinceCode, string districtCode, string wardCode, string address)
        {
            var json = _tplemsRepository.InsertInventory(name, username, phone, provinceCode, districtCode, wardCode, address);
            return JsonSuccess(json);
        }
        [AllowAnonymous]
        [HttpGet("CreateShipment")]
        public async Task<JsonResult> CreateShipment(CreateShipment model)
        {
            var json = _tplemsRepository.CreateShipment(model);
            return JsonSuccess(json);
        }
    }
}