using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace Jpp.Files.Api.TelemetryFilters
{
    internal class HealthCheckFilter : ITelemetryProcessor
    {
        private const string HEALTH_CHECK_REQUEST = @"get /healthcheck";
        private readonly ITelemetryProcessor _next;

        public HealthCheckFilter(ITelemetryProcessor next)
        {
            _next = next;
        }

        public void Process(ITelemetry item)
        {
            if (item is RequestTelemetry request)
            {
                if (request.Name.ToLower().Equals(HEALTH_CHECK_REQUEST)) return;
            }

            _next.Process(item);
        }
    }
}
