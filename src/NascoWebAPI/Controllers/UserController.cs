using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
//using NascoWebAPI.Models.UserViewModels;
using System.Net.Http;
using Microsoft.AspNetCore.Http;
using System.Web.Http;
using NascoWebAPI.Services.Interface;
using NascoWebAPI.Data;
using NascoWebAPI.Helper.JwtBearerAuthentication;
using Microsoft.Extensions.Options;
using NascoWebAPI.Models;
using Newtonsoft.Json.Linq;
using System.Reflection;
using NascoWebAPI.Helper;
using NascoWebAPI.Helper.Common;

namespace NascoWebAPI.Controllers
{

    [Route("api/user")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserServices _userServices;
        private readonly IOfficerRepository _officeRepository;
        private readonly JwtIssuerOptions _jwtOptions;
        public UserController(ILogger<UserController> logger,
            IUserServices userServices,
            IOfficerRepository officeRepository,
            IOptions<JwtIssuerOptions> jwtOptions) : base(logger)
        {
            _userServices = userServices;
            _officeRepository = officeRepository;
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }
        [HttpPost("SignIn")]
        [AllowAnonymous]
        public async Task<JsonResult> SignIn([FromBody]LoginModel model)
        {
            if (model != null)
            {
                if (string.IsNullOrEmpty(model.UserName) || string.IsNullOrEmpty(model.Password))
                    return JsonError("Username and password can not be empty!");

                var result = await _userServices.SignIn(_officeRepository, model);
                _logger.LogInformation($"SignIn: user {model.UserName}");
                return Json(result);
            }
            return JsonError("null");
        }

        [HttpPost("UpdatePosition")]
        public async Task<JsonResult> UpdatePosition([FromBody]JObject jsonData)
        {
            if (jsonData != null)
            {
                dynamic json = jsonData;
                string Lat = json.lat;
                string Lng = json.lng;

                if (!string.IsNullOrEmpty(Lat) || !string.IsNullOrEmpty(Lng))
                {
                    var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
                    var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
                    if (user != null)
                    {
                        user.LatDynamic = Lat;
                        user.LngDynamic = Lng;
                        user.LocationTime = System.DateTime.Now;
                        _logger.LogInformation($"Update Lat, Lng : user {user.UserName}");
                        _officeRepository.SaveChanges();
                        user.Password = null;
                        return Json(user);
                    }
                }
                else
                {
                    return JsonError("lat, lng is not empty");
                }

            }
            return JsonError("null");
        }
        [HttpGet("Get")]
        public async Task<JsonResult> Get()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                return Json(user);
            }
            return JsonError("User not found");
        }
        [HttpPost("Update")]
        public async Task<JsonResult> Update([FromBody]JObject jsonData)
        {
            if (jsonData != null)
            {
                dynamic json = jsonData;

                var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
                var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
                if (user != null)
                {
                    user.CopyFromJOject(jsonData);
                    _logger.LogInformation($"Update user : user {user.UserName}");
                    _officeRepository.SaveChanges();
                    user.Password = null;
                    return Json(user);
                }
                JsonError("User not found");
            }
            return JsonError("null");
        }
        [HttpPost("ChangePassWord")]
        public async Task<JsonResult> ChangePassWord([FromBody]JObject jsonData)
        {
            if (jsonData != null)
            {
                dynamic json = jsonData;

                var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
                var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
                if (user != null)
                {
                    if (user.Password != Command.EncryptString(json.passwordOld.ToString()))
                    {
                        return JsonError("Wrong PassWord!");
                    }
                    user.Password = Command.EncryptString(json.passwordNew.ToString());
                    _logger.LogInformation($"Update user : user {user.UserName}");
                    _officeRepository.SaveChanges();
                    user.Password = null;
                    return Json(user);
                }
                JsonError("User not found");
            }
            return JsonError("null");
        }
        [HttpGet("GetBadgeMenu")]
        public async Task<JsonResult> GetBadgeMenu()
        {
            var jwtDecode = JwtDecode(Request.Headers["Authorization"].ToString().Split(' ')[1]);
            var user = await _officeRepository.GetSingleAsync(o => o.UserName == jwtDecode.Subject);
            if (user != null)
            {
                return Json(_officeRepository.GetListMenuByOfficer(user.OfficerID));
            }
            return JsonError("User not found");
        }
    }

}
