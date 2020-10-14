using System;
using System.Net.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Jpp.MessageBroker;

namespace Jpp.Email.Service
{
    internal class Message
    {
        private readonly MailAddress _toAddress;
        private readonly MailAddress _fromAddress;
        private readonly string _subject;
        private readonly string _body;
        private readonly Guid? _attachmentGuid;
        private readonly SmtpClient _smtpClient;
        private readonly HttpClient _httpClient;

        public static Message Create(EmailServiceMessage message, string sender, SmtpClient smtpClient, HttpClient httpClient)
        {
            return new Message(sender, message.EmailAddress, message.Subject, message.Body, message.AttachmentGuid, smtpClient, httpClient);
        }

        private Message(string sender, string recipient, string subject, string body, Guid? attachmentGuid, SmtpClient smtpClient, HttpClient httpClient)
        {
            _subject = subject;
            _body = body;
            _attachmentGuid = attachmentGuid;
            _toAddress = new MailAddress(recipient);
            _fromAddress = new MailAddress(sender);
            _smtpClient = smtpClient;
            _httpClient = httpClient;
        }

        public async Task SendAsync()
        {   
            using (var message = new MailMessage(_fromAddress.Address, _toAddress.Address, _subject, _body))
            {
                if (_attachmentGuid != null) message.Attachments.Add(await GetAttachmentAsync(_attachmentGuid.Value));
                await _smtpClient.SendMailAsync(message);
            }
        }

        private async Task<Attachment> GetAttachmentAsync(Guid attachmentGuid)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"api/files/{attachmentGuid.ToString()}"))
            {
                var response = await _httpClient.SendAsync(request);
                var contentStream = await response.Content.ReadAsStreamAsync();

                return new Attachment(contentStream, new ContentType(response.Content.Headers.ContentType.MediaType));
            }
        }
    }
}
