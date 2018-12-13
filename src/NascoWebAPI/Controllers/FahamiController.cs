using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Client;

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Fahami")]
    [AllowAnonymous]
    public class FahamiController : BaseController
    {
        private readonly IFahamiClient _iFahamiClient;

        public FahamiController(ILogger<UserController> logger,
            IFahamiClient iFahamiClient
            ) : base(logger)
        {
            _iFahamiClient = iFahamiClient;
        }
        #region [Get]
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetAll()
        {
            var result = await _iFahamiClient.GetProductTypes();
            return Json(result);
        }
        #endregion
    }
}
