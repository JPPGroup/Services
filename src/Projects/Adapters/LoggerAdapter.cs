using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;

namespace Jpp.Web.Service.Adapters
{
    public class LoggerAdapter<T> : ILoggerAdapter<T>
    {
        private readonly ILogger<T> logger;

        public LoggerAdapter(ILogger<T> logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [ExcludeFromCodeCoverage]
        public void LogError(Exception ex, string message, params object[] args)
        {
            logger.LogError(ex, message, args);
        }

        [ExcludeFromCodeCoverage]
        public void LogError(string message)
        {
            logger.LogError(message);
        }
    }
}
