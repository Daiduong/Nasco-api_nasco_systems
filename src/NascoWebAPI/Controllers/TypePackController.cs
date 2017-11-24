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
    [Route("api/TypePack")]
    [AllowAnonymous]
    public class TypePackController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ITypePackRepository _TypePackRepository;
        public TypePackController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ITypePackRepository TypePackRepository
            ) : base(logger)
        {
            _TypePackRepository = TypePackRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetAll()
        {
            var result = await _TypePackRepository.GetAllAsync();
            return Json(result);
        }
        #endregion
    }
}