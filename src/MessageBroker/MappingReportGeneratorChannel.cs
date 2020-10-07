using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Jpp.MessageBroker
{
    public class MappingReportGeneratorChannel : IMessageChanel<MappingReportGeneratorMessage>
    {
        ConnectionFactory _factory;
        IConnection _connection;
        IModel _channel;
        IBasicProperties _properties;
        EventingBasicConsumer _consumer;

        private BasicDeliverEventArgs receivedMessage;
        private AutoResetEvent receivedReset;

        public MappingReportGeneratorChannel(bool Receive = false)
        {
            _factory = new ConnectionFactory() { HostName = GlobalConstants.HOST_NAME };
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

            if (Receive)
            {
                _consumer = new EventingBasicConsumer(_channel);
                _consumer.Received += (model, ea) =>
                {
                    receivedMessage = ea;
                    receivedReset.Set();
                };
                _channel.BasicConsume(queue: "mapping_report_generator_task_queue",
                                     autoAck: false,
                                     consumer: _consumer);
            }

            receivedReset = new AutoResetEvent(false);
        }


        public async Task SendMessageAsync(MappingReportGeneratorMessage message)
        {        
            await Task.Run(() =>
            {
                string json = JsonConvert.SerializeObject(message);
                byte[] body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(exchange: "",
                                routingKey: "mapping_report_generator_task_queue",
                                basicProperties: _properties,
                                body: body);
            });

        }

        public async Task<MappingReportGeneratorMessage> ReceiveMessageAsync()
        {
            //TODO: Verify this behaves as expcted, and no issues with while loop
            while (receivedMessage == null)
            {
                receivedReset.WaitOne();
            }

            var body = receivedMessage.Body;
            var message = Encoding.UTF8.GetString(body);
            return JsonConvert.DeserializeObject<MappingReportGeneratorMessage>(message);                    
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

    public struct MappingReportGeneratorMessage
    {
        public string Email;
        public string Client;
        public string Project;
        public double Latitude;
        public double Longitude;
    }
}
