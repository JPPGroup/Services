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
    public class GenerateRequestSendChannel : ISendChannel<GenerateRequestMessage>
    {
        ConnectionFactory _factory;
        IConnection _connection;
        IModel _channel;
        IBasicProperties _properties;
        EventingBasicConsumer _consumer;
        private ILogger<ISendChannel<GenerateRequestMessage>> _logger;

        private BasicDeliverEventArgs receivedMessage;
        private AutoResetEvent receivedReset;

        public GenerateRequestSendChannel(IConfiguration config, ILogger<ISendChannel<GenerateRequestMessage>> logger)
        {
            _logger = logger;
            _logger.LogInformation("Creating channel");

            _factory = new ConnectionFactory() { HostName = config["MBHOST"], UserName = config["MBUSER"], Password = config["MBPASS"] };
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

            receivedReset = new AutoResetEvent(false);
            _logger.LogInformation("Channel open");
        }


        public async Task SendMessageAsync(GenerateRequestMessage message)
        {        
            await Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(exchange: "",
                                routingKey: "mapping_report_generator_task_queue",
                                basicProperties: _properties,
                                body: body);
                _logger.LogTrace("Message sent");
            });

        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();            
        }
    }

    
}
