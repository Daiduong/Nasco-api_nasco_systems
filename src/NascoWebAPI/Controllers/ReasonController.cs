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
    [Route("api/Reason")]
    [AllowAnonymous]
    public class ReasonController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IReasonRepository _reasonRepository;
        public ReasonController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IReasonRepository reasonRepository
            ) : base(logger)
        {
            _reasonRepository = reasonRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetListReasonMAWB")]
        public async Task<JsonResult> GetListReasonMAWB()
        {
            return JsonSuccess(await _reasonRepository.GetAsync(x => x.TableName == "MAWB" && x.State == 0));
        }
        #endregion
    }
}