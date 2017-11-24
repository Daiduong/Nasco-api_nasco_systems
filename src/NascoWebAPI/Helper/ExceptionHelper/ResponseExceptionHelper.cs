using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net;
using System.Collections.Generic;

namespace NascoWebAPI.Helper.ExceptionHelper
{
    public static class ResponseExceptionHelper
    {
        public static async Task HandleExceptionAsync(HttpContext conText, Exception Exception)
        {
            if (Exception == null) return;

            var code = HttpStatusCode.InternalServerError;

            if (Exception is KeyNotFoundException) code = HttpStatusCode.NotFound;
            else if (Exception is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;
            else if (Exception is Exception) code = HttpStatusCode.BadRequest;

            await WriteExceptionAsync(conText, Exception, code).ConfigureAwait(false);
        }

        private static async Task WriteExceptionAsync(HttpContext conText, Exception Exception, HttpStatusCode code)
        {
            var response = conText.Response;
            await response.WriteAsync(JsonConvert.SerializeObject(new
            {
                isSuccess = false,
                Exception = new
                {
                    type = Exception.GetType().Name,
                    message = Exception.Message,
                    stracktrace = Exception.StackTrace
                }
            })).ConfigureAwait(false);
        }
    }
}
