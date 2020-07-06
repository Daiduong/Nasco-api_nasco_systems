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
    [Route("api/UnitGroup")]
    [Authorize]
    public class UnitGroupController : BaseController
    {
        private readonly IUnitGroupRepository _unitGroupRepository;
        public UnitGroupController(ILogger<UserController> logger,
           IUnitGroupRepository UnitGroupRepository
           ) : base(logger)
        {
            _unitGroupRepository = UnitGroupRepository;
        }

        [AllowAnonymous]
        [HttpGet("GetAllUnitGroup")]
        public async Task<JsonResult> GetAllUnitGroup()
        {
            var json = _unitGroupRepository._GetAllUnitGroup();
            return JsonSuccess(json);
        }
    }
}