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
    [Route("api/Customer")]
    [Authorize]
    public class CustomerController : BaseController
    {
        private readonly IOfficerRepository _officeRepository;
        private readonly ICustomerRepository _customerRepository;
        public CustomerController(ILogger<UserController> logger,
            IOfficerRepository officeRepository,
            ICustomerRepository customerRepository
            ) : base(logger)
        {
            _customerRepository = customerRepository;
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
                var result = await _customerRepository.GetSingleAsync(c => c.CustomerID == id);
                if (result == null)
                {
                    return JsonError("Not Found");
                }
                return Json(result);
            }
            return JsonError("null");
        }
        [HttpGet("GetAll")]
        public async Task<JsonResult> GetAll()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                var result = await _customerRepository.GetAllAsync();
                return Json(result);
            }
            return JsonError("null");
        }
        #endregion
    }
}