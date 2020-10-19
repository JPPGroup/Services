using System;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jpp.MessageBroker.Generics;
using Jpp.MessageBroker.Mapping;
using Microsoft.Extensions.Logging;

namespace Jpp.MessageBroker
{
    public class GenerateRequestReceiveChannel : IReceiveChannel<GenerateRequestMessage>
    {
        ConnectionFactory _factory;
        IConnection _connection;
        IModel _channel;
        IBasicProperties _properties;
        EventingBasicConsumer _consumer;
        private ILogger<IReceiveChannel<GenerateRequestMessage>> _logger;

        private BasicDeliverEventArgs receivedMessage;
        private AutoResetEvent receivedReset;

        public GenerateRequestReceiveChannel(IConfiguration config, ILogger<IReceiveChannel<GenerateRequestMessage>> logger)
        {
            _logger = logger;
            _logger.LogInformation("Creating channel");

            _factory = new ConnectionFactory() { HostName = config["MBHOST"], UserName = config["MBUSER"], Password = config["MBPASS"]};
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(queue: "mapping_report_generator_task_queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

            _properties = _channel.CreateBasicProperties();
            _properties.Persistent = true;

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                _logger.LogTrace("Message received");
                receivedMessage = ea;
                receivedReset.Set();
            };
            _channel.BasicConsume(queue: "mapping_report_generator_task_queue",
                autoAck: false,
                consumer: _consumer);


            receivedReset = new AutoResetEvent(false);
            _logger.LogInformation("Channel listening");
        }

        public async Task<GenerateRequestMessage> ReceiveMessageAsync(CancellationToken cancellationToken)
        {
            //TODO: Verify this behaves as expcted, and no issues with while loop
            await Task.Run(async () =>
            {
                while (receivedMessage == null && !cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000);
                }
                _logger.LogTrace("Async released");

            }, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            var body = receivedMessage.Body;
            var message = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<GenerateRequestMessage>(message);                    
        }

        public void RequestComplete()
        {
            _channel.BasicAck(deliveryTag: receivedMessage.DeliveryTag, multiple: false);
            receivedMessage = null;
        }

        public void RequestFailed()
        {
            _channel.BasicReject(deliveryTag: receivedMessage.DeliveryTag, requeue: true);
            receivedMessage = null;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();            
        }
    }

    
}
