using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.JwtBearerAuthentication;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace NascoWebAPI.Controllers
{
    public class BaseController : Controller
    {
        public ILogger<dynamic> _logger;

        public BaseController(ILogger<dynamic> logger)
        {
            _logger = logger;
        }
        #region Response Data
        public dynamic Success()
        {
            return new { errorMessage = (string)null };
        }

        public dynamic Success(ExpandoObject obj)
        {
            obj.Append(new KeyValuePair<string, object>("errorMessage", null));
            return obj;
        }

        public dynamic Error(string message)
        {
            return new { errorMessage = message };
        }

        public JsonResult JsonSuccess()
        {
            return Json(new { errorMessage = (string)null });
        }

        public JsonResult JsonSuccess(ExpandoObject obj)
        {
            obj.Append(new KeyValuePair<string, object>("errorMessage", null));
            return Json(obj);
        }
        public JsonResult JsonSuccess(object data = null, string message = "")
        {
            return Json(new { data, message, error = 0 });
        }
        public JsonResult JsonError(string message)
        {
            return Json(new { errorMessage = message });
        }
        #endregion

        public JwtSecurityToken JwtDecode(string Jwttoken)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(Jwttoken);
        }

        public void ThrowIfInvalidOptions(JwtIssuerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            if (options.ValidFor <= TimeSpan.Zero)
            {
                throw new ArgumentException("Must be a non-zero TimeSpan.", nameof(JwtIssuerOptions.ValidFor));
            }

            if (options.SigningCredentials == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.SigningCredentials));
            }

            if (options.JtiGenerator == null)
            {
                throw new ArgumentNullException(nameof(JwtIssuerOptions.JtiGenerator));
            }
        }
    }
}
