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
    [Route("api/Structure")]
    [AllowAnonymous]
    public class StructureController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IStructureRepository _structureRepository;
        public StructureController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IStructureRepository StructureRepository
            ) : base(logger)
        {
            _structureRepository = StructureRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetListByTransport")]
        public async Task<JsonResult> GetListByTransport(int transportId)
        {
            var result = await _structureRepository.GetListByTransport(transportId);
            return Json(result);
        }
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetList()
        {
            var result = await _structureRepository.GetAsync( x=> x.State == 0);
            return Json(result);
        }
        #endregion
    }
}