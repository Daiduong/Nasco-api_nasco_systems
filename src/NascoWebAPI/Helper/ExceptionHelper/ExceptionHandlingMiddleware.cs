using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper.ExceptionHelper
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext HttpContext)
        {
            try
            {
                await _next(HttpContext);
            }
            catch (Exception ex)
            {
                await ResponseExceptionHelper.HandleExceptionAsync(HttpContext, ex);
            }
        }
    }
}
