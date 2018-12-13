using Microsoft.Extensions.Logging;
using NascoWebAPI.Helper.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper.Logging
{
    public class LoggingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;
        public LoggingHandler(HttpMessageHandler innerHandler)
            : base(innerHandler)
        {
            _logger = ApplicationLogging.CreateLogger("LoggingHandler");
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var log = new StringBuilder();
            log.AppendLine("REQUEST:");
            log.AppendLine(request.ToString());
            if (request.Content != null)
            {
                log.AppendLine(await request.Content.ReadAsStringAsync());
            }
            log.AppendLine();

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            log.AppendLine("RESPONSE:");
            log.AppendLine(response.ToString());
            if (response.Content != null)
            {
                log.AppendLine(await response.Content.ReadAsStringAsync());
            }
            log.AppendLine();
            _logger.LogInformation(log.ToString());
            return response;
        }
    }
}
