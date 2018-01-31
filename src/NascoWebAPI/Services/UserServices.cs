using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using NascoWebAPI.Services.Interface;
using Microsoft.Extensions.Options;
using NascoWebAPI.Helper;
//using NascoWebAPI.Models.UserViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NascoWebAPI.Models;
using NascoWebAPI.Data;
using NascoWebAPI.Helper.JwtBearerAuthentication;

namespace NascoWebAPI.Services
{
    public partial class UserServices : BaseServices, IUserServices
    {
        private readonly JwtIssuerOptions _jwtOptions;

        public UserServices(
            ILogger<UserServices> logger,
            IMapper mapper,
            IOptions<JwtIssuerOptions> jwtOptions) : base(logger, mapper)
        {
            _jwtOptions = jwtOptions.Value;
            ThrowIfInvalidOptions(_jwtOptions);
        }

        public async Task<dynamic> SignIn(IOfficerRepository officer, LoginModel loginModel)
        {
            var identity = await GetClaimsIdentity(officer, loginModel);
            if (identity == null)
            {
                _logger.LogInformation($"Invalid username or password");
                return new
                {
                    errorMessage = $"Invalid username or password"
                };
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, loginModel.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, await _jwtOptions.JtiGenerator()),
                new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(_jwtOptions.IssuedAt).ToString(), ClaimValueTypes.Integer64),
            };

            // Create the JWT security token and encode it.
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: _jwtOptions.NotBefore,
                expires: _jwtOptions.Expiration,
                signingCredentials: _jwtOptions.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            var user = await officer.GetSingleAsync(o => o.UserName == loginModel.UserName);

            // Serialize and return the response
            return new
            {
                userId = user.OfficerID.ToString(),
                userFullName = user.OfficerName,
                jobId = user.JobId,
                departmentId = user.DeparmentID,
                token = encodedJwt,
                expires = (int)_jwtOptions.ValidFor.TotalSeconds,
                errorMessage = (string)null
            };

        }
    }
}
