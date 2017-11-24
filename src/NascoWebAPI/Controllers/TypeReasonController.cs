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
    [Route("api/TypeReason")]
    [Authorize]
    public class TypeReasonController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ITypeReasonRepository _typeReasonRepository;
        public TypeReasonController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ITypeReasonRepository typeReasonRepository
            ) : base(logger)
        {
            _typeReasonRepository = typeReasonRepository;
            _officeRepository = officeRepository;
        }
        #region [Get]
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetAll()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _typeReasonRepository.GetAllAsync();
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetSingle")]
        public async Task<JsonResult> GetSingle(int id)
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _typeReasonRepository.GetSingleAsync(c => c.TypeReasonID == id);
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        #endregion
    }
}