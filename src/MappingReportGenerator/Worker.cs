using System;
using System.Threading;
using System.Threading.Tasks;
using Jpp.MessageBroker.Generics;
using Jpp.MessageBroker.Mapping;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jpp.MappingReportGenerator
{
    class Worker : BackgroundService
    {
        private ILogger<Worker> _logger;
        private IReceiveChannel<GenerateRequestMessage> _messageChannel;

        public Worker(ILogger<Worker> logger, IReceiveChannel<GenerateRequestMessage> messageChanel)
        {
            _logger = logger;
            _messageChannel = messageChanel;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Work loop started");
            TileProvider _provider = new TileProvider();

            while (!stoppingToken.IsCancellationRequested)
            {
                GenerateRequestMessage workItem = await _messageChannel.ReceiveMessageAsync(stoppingToken);
                DateTime start = DateTime.Now;

                _logger.LogInformation($"Request received, generating new standard report {workItem.Id}");
                using (MappingReport.CreateStandard(_provider, workItem.Project, workItem.Client, workItem.Id,
                    new WGS84(workItem.Latitude, workItem.Longitude)))
                {
                    int i = 0;
                }

                GC.Collect();
                _messageChannel.RequestComplete();

                DateTime end = DateTime.Now;
                TimeSpan duration = end - start;
                _logger.LogInformation($"Report {workItem.Id} finished in {duration.TotalSeconds}s");
            }
        }
    }
}
