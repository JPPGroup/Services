using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jpp.MessageBroker
{
    public class EmailServiceChannel : IChannel<EmailServiceMessage>
    {
        private const string QUEUE_NAME = "email_service_task_queue";

        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly IBasicProperties _properties;
        private readonly EventingBasicConsumer _consumer;        
        private readonly AutoResetEvent _receivedReset;

        private BasicDeliverEventArgs _receivedMessage;

        public EmailServiceChannel()
        {
            _factory = new ConnectionFactory() { HostName = GlobalConstants.HOST_NAME };
            _connection = _factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare(QUEUE_NAME, true,  false, false, null);
            _channel.BasicQos(0,  1, false);
         
            _properties = _channel.CreateBasicProperties();
            _properties.ContentType = "application/json";
            _properties.Persistent = true;

            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += (model, ea) =>
            {
                _receivedMessage = ea;
                _receivedReset.Set();
            };

            _channel.BasicConsume(QUEUE_NAME, false, _consumer);

            _receivedReset = new AutoResetEvent(false);
        }

        public async Task SendMessageAsync(EmailServiceMessage message)
        {        
            await Task.Run(() =>
            {
                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish("", QUEUE_NAME, _properties, body);
            });
        }

        public async Task<EmailServiceMessage> ReceiveMessageAsync(CancellationToken cancellationToken)
        {
            if (_receivedMessage == null) _receivedReset.WaitOne();

            var body = _receivedMessage?.Body;
            var message = Encoding.UTF8.GetString(body ?? throw new InvalidOperationException());

            return await Task.FromResult(JsonConvert.DeserializeObject<EmailServiceMessage>(message));                    
        }

        public void RequestComplete()
        {
            _channel.BasicAck(_receivedMessage.DeliveryTag, false);
            _receivedMessage = null;
        }

        public void RequestFailed()
        {
            _channel.BasicReject(_receivedMessage.DeliveryTag, true);
            _receivedMessage = null;
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();            
        }
    }

    public struct EmailServiceMessage
    {
        public string EmailAddress;
        public Guid? AttachmentGuid;
        public string Subject;
        public string Body;
    }
}
