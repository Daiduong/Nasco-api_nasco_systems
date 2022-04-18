using System;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using NascoWebAPI.Helper;
using Microsoft.AspNetCore.Identity;
using NascoWebAPI.Models;
using NascoWebAPI.Helper.JwtBearerAuthentication;
using NascoWebAPI.Data;
using NascoWebAPI.Helper.Common;
//using NascoWebAPI.Models.UserViewModels;;

namespace NascoWebAPI.Services
{
    public partial class UserServices
    {
        private void ThrowIfInvalidOptions(JwtIssuerOptions options)
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
        /// <returns>Date converted to seconds since Unix epoch (Jan 1, 1970, midnight UTC).</returns>
        private static long ToUnixEpochDate(DateTime date)
          => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
        /// <summary>
        /// IMAGINE BIG RED WARNING SIGNS HERE!
        /// You'd want to retrieve claims through your claims provider
        /// in whatever way suits you, the below is purely for demo purposes!
        /// </summary>
        private async Task<ClaimsIdentity> GetClaimsIdentity(IOfficerRepository office, LoginModel loginModel)
        {
            loginModel.PasswordHash = Command.EncryptString(loginModel.Password);
            var user = await office.GetSingleAsync(o => o.UserName == loginModel.UserName && o.Password == loginModel.PasswordHash && o.State == 0);
            if (user != null)
            {
                user.LocationTime = System.DateTime.Now;
                office.SaveChanges();
                return await Task.FromResult(new ClaimsIdentity(new GenericIdentity(user.OfficerName, "Token"),
                  new[]
                  {
                     new Claim("userId",user.OfficerID.ToString()),
                     new Claim("userFullName", user.OfficerName)
                  }
                  ));
            }

            // Credentials are invalid, or account doesn't exist
            return await Task.FromResult<ClaimsIdentity>(null);
        }
    }
}
