using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NascoWebAPI.Helper.Logger
{
    public class ApplicationLogging
    {
        private static ILoggerFactory _Factory = null;

        public static void ConfigureLogger(ILoggerFactory factory)
        {
            factory.AddDebug(LogLevel.None);
            factory.AddSerilog();
        }

        public static ILoggerFactory LoggerFactory
        {
            get
            {
                if (_Factory == null)
                {
                    _Factory = new LoggerFactory();
                    ConfigureLogger(_Factory);
                }
                return _Factory;
            }
            set { _Factory = value; }
        }
        public static Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);

    }
}
