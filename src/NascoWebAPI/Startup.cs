using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using NascoWebAPI.Data;
using Serilog;
using System.IO;
using Microsoft.IdentityModel.Tokens;
using NascoWebAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NascoWebAPI.Models.JwtModels;
using System.Net.Http.Formatting;
using Newtonsoft.Json.Serialization;
using NascoWebAPI.Helper.Logger;
using Microsoft.AspNetCore.Identity;

namespace NascoWebAPI
{
    public partial class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                //builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            EnvName = env.EnvironmentName;

        }

        private IConfigurationRoot Configuration { get; }

        private string EnvName { get; set; }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Helper.ConnectionHelper.CONNECTION_STRING = Configuration.GetConnectionString("NascoWebAPIConnection");
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);
           
            services.Configure<FormOptions>(options => options.BufferBody = true);
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Helper.ConnectionHelper.CONNECTION_STRING));
            
            services.AddIdentity<ApplicationUser, IdentityRole>()
               .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddCors();

            AutoMapperCfg(services);
            JwtConfigService(services);
            MappingScopeService(services);

            // Add application services.
            services.Configure<JwtModel>(Configuration.GetSection("Jwt"));
            services.AddMvc()
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver())
                .AddJsonOptions(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Log.Logger = new LoggerConfiguration()
                                            .MinimumLevel.Debug()
                                            .WriteTo.RollingFile(Path.Combine(env.ContentRootPath, $"logs\\{env.EnvironmentName.ToLowerInvariant()}-log-{ DateTime.Now.ToString("dd-MM-yyyy") }.txt"))
                                            .CreateLogger();
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            ApplicationLogging.ConfigureLogger(loggerFactory);
            ApplicationLogging.LoggerFactory = loggerFactory;

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
     
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });
            app.UseApplicationInsightsExceptionTelemetry();
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseIdentity();

            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            JwtConfig(app, loggerFactory);
            MiddlewareConfig(app, loggerFactory);
            // Add external authentication middleware below. To configure them please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc();

            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute(
            //        name: "default",
            //        template: "{controller=User}/{action=SignIn}/{id?}");
            //});

        }
    }
}
