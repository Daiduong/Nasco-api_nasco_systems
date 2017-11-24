using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NascoWebAPI.Data;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NascoWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Auto")]
    public class AutoController : BaseController
    {
        private IOfficerRepository _iOfficeRepository;

        public AutoController(Microsoft.Extensions.Logging.ILogger<dynamic> logger,
            IOfficerRepository iOfficeRepository) : base(logger)
        {
            _iOfficeRepository = iOfficeRepository;
        }
        [AllowAnonymous]
        [HttpPost("SendNotification")]
        public async Task<JsonResult> SendNotification([FromBody]NotificationObject obj)
        {

            return Json(await GCM.SendNotification(obj.token, obj.message, obj.badge));
        }

        public class NotificationObject
        {
            public string token { get; set; }
            public string message { get; set; }
            public int badge { get; set; }
        }
    }
}
