using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.ExceptionHelper;

namespace NascoWebAPI
{
    public partial class Startup
    {
        private void MiddlewareConfig(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
        }

    }
}
