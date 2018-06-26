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
    [Route("api/Recipient")]
    [Authorize]
    public class RecipientController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly IRecipientRepository _recipientRepository;
        public RecipientController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            IRecipientRepository recipientRepository
            ) : base(logger)
        {
            _recipientRepository = recipientRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetSingle")]
        public async Task<JsonResult> GetSingle(int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _recipientRepository.GetSingleAsync(c => c.Id == id);
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("Search")]
        public async Task<JsonResult> Search(string s)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null && !string.IsNullOrWhiteSpace(s))
            {
                var result = await _recipientRepository.GetAsync(c => c.State == 0 && ((c.Phone != null && c.Phone.Contains(s))
                                   || (c.Phone2 != null && c.Phone2.Contains(s))
                                   || (c.Name != null && c.Name.ToUpper().Contains(s.ToUpper())))
                                   , null, null, 10
                                   , inc => inc.City, inc => inc.District);
                return Json(result);
            }
            return JsonError("null");
        }
        #endregion
    }
}