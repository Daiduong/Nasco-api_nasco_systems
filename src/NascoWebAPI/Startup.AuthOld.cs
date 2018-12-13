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

namespace NascoWebAPI
{
    public partial class Startup
    {
        ////private const string SecretKey = "08xPOnBQewKuCYJKg1AsHjA5Pp4B4eJ08ZyWkKNJhRTELNKf06reQFdvE_KW822G";
        ////private readonly SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));
        //private void JwtConfigService(IServiceCollection services)
        //{
        //    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:SecretKey"]));

        //    // Add framework services.
        //    services.AddOptions();

        //    // Make authentication compulsory across the board (i.e. shut
        //    // down EVERYTHING unless explicitly opened up).
        //    services.AddMvc(config =>
        //    {
        //        var policy = new AuthorizationPolicyBuilder()
        //                         .RequireAuthenticatedUser()
        //                         .Build();
        //        config.Filters.Add(new AuthorizeFilter(policy));
        //       // config.Filters.Add(new RequireHttpsAttribute());
        //        config.RespectBrowserAcceptHeader = true;
        //        config.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
        //    })
        //    .AddJsonOptions(jsonOptions =>
        //    {
        //        //Suppress properties with null value
        //        //jsonOptions.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
        //    });

        //    // Use policy auth.
        //    services.AddAuthorization(
        //     options =>
        //     {
        //         //options.AddPolicy("DisneyUser", policy => policy.RequireClaim("DisneyCharacter", "IAmMickey"));
        //     });
        //    services.AddAuthentication(options => options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme);
        //    // Get options from app settings
        //    var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

        //    // Configure JwtIssuerOptions
        //    services.Configure<JwtIssuerOptions>(options =>
        //    {
        //        options.Issuer = Configuration["Authentication:JwtIssuerOptions:Issuer"];
        //        options.Audience = Configuration["Authentication:JwtIssuerOptions:Audience"];
        //        options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        //    });
        //}
        //private void JwtConfig(IApplicationBuilder app, ILoggerFactory loggerFactory)
        //{
        //    var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["Jwt:SecretKey"]));

        //    var audience = Configuration["Authentication:JwtIssuerOptions:Audience"];
        //    app.UseJwtBearerAuthentication(new JwtBearerOptions
        //    {
        //        AutomaticAuthenticate = true,
        //        AutomaticChallenge = true,
        //        TokenValidationParameters = new TokenValidationParameters
        //        {
        //            ValidateIssuer = true,
        //            ValidIssuer = Configuration["Authentication:JwtIssuerOptions:Issuer"],

        //            ValidateAudience = true,
        //            ValidAudience = Configuration["Authentication:JwtIssuerOptions:Audience"],

        //            ValidateIssuerSigningKey = true,
        //            IssuerSigningKey = signingKey,

        //            RequireExpirationTime = true,
        //            ValidateLifetime = true,

        //            ClockSkew = TimeSpan.Zero
        //        }
        //    });
        //    app.UseCookieAuthentication(new CookieAuthenticationOptions
        //    {
        //        AuthenticationScheme = "Cookies",
        //        AutomaticAuthenticate = true,
        //        AutomaticChallenge = true
        //    });
        //    //app.UseFacebookAuthentication(new FacebookOptions()
        //    //{
        //    //    AppId = Configuration["Authentication:Facebook:AppId"],
        //    //    AppSecret = Configuration["Authentication:Facebook:AppSecret"]
        //    //});
        //    //app.UseGoogleAuthentication(new GoogleOptions()
        //    //{
        //    //    ClientId = Configuration["Authentication:Google:ClientId"],//dev.logistic.vlp@gmail.com /0nemoretimes
        //    //    ClientSecret = Configuration["Authentication:Google:ClientSecret"]
        //    //});

        //}
    }
}
