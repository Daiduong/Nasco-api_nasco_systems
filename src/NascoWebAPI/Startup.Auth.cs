using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using NascoWebAPI.Helper;
using Microsoft.AspNetCore.Mvc.Formatters;
using NascoWebAPI.Helper.JwtBearerAuthentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace NascoWebAPI
{
    public partial class Startup
    {
        private void JwtConfigService(IServiceCollection services)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:SecretKey"]));
            var issurer = Configuration["Authentication:JwtIssuerOptions:Issuer"];
            var audience = Configuration["Authentication:JwtIssuerOptions:Audience"];

            var tokenValidationParameters = new TokenValidationParameters
            {
                //The signing key must match !
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                //Validate the JWT Issuer (iss) claim
                ValidateIssuer = true,
                ValidIssuer = issurer,

                //validate the JWT Audience (aud) claim

                ValidateAudience = true,
                ValidAudience = audience,

                //validate the token expiry
                ValidateLifetime = true,

                //
                RequireExpirationTime = true,

                // If you  want to allow a certain amout of clock drift
                ClockSkew = TimeSpan.Zero

            };

            // Make authentication compulsory across the board (i.e. shut
            // down EVERYTHING unless explicitly opened up).
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                                 .RequireAuthenticatedUser()
                                 //.AddRequirements(new PermissionsRequirement())
                                 .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
                // config.Filters.Add(new RequireHttpsAttribute());
                config.RespectBrowserAcceptHeader = true;
                config.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            })
            .AddJsonOptions(jsonOptions =>
            {
                //Suppress properties with null value
                //jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
            });

            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = tokenValidationParameters;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = "/Account/Forbidden/";
                    options.LoginPath = "/Account/Unauthorized/";
                });
            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = issurer;
                options.Audience = audience;
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });
        }

        private void JwtConfig(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {

        }
    }
}
