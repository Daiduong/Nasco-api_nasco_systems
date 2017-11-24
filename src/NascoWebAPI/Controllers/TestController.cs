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
    [Route("api/Test")]
    [AllowAnonymous]
    public class TestController : BaseController
    {
        public TestController(ILogger<UserController> logger
            ) : base(logger)
        {
        }
        #region [Get]
        [HttpGet("Get")]
        public JsonResult Get()
        {
            return JsonError("Get");
        }
        [HttpPost("Post")]
        public JsonResult Post()
        {
            return JsonError("Post");
        }
        #endregion
    }
}