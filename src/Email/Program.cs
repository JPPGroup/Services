using System;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Jpp.MessageBroker;
using Microsoft.Extensions.Configuration;

namespace Jpp.Email.Service
{
    internal class Program
    {
        public static IConfiguration Configuration { get; private set; }

        public static void Main(string[] args)
        {
            Configuration = CreateConfiguration();

            var smtpClient = CreateSmtpClient();
            var httpClient = CreateHttpClient();
            var sender = Configuration["SENDER_ADDRESS"];

            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var backgroundTask = Task.Run(async () =>
            {
                var capturedToken = token;

                using (IMessageChanel<EmailServiceMessage> channel = new EmailServiceChannel())
                {
                    while (!capturedToken.IsCancellationRequested)
                    {
                        var workMessage = await channel.ReceiveMessageAsync();
                        Console.WriteLine(@"Message received");
                        try
                        {
                            await Message.Create(workMessage, sender, smtpClient, httpClient).SendAsync();
                            channel.RequestComplete();
                            Console.WriteLine(@"Email Sent");                            
                        }
                        catch (Exception ex)
                        {
                            channel.RequestFailed();
                            Console.WriteLine($@"Email Failed - {ex}");                            
                            //TODO: Need to log...
                        }
                    }
                }
            }, token);

            Console.CancelKeyPress += (o, e) =>
            {
                tokenSource.Cancel();
                e.Cancel = true;
            };

            backgroundTask.Wait(token);
        }

        private static IConfiguration CreateConfiguration()
        {
            var config = new ConfigurationBuilder();
            config.AddEnvironmentVariables("EMAIL_");

            return config.Build();
        }

        private static SmtpClient CreateSmtpClient()
        {
            return new SmtpClient
            {
                Host = Configuration["SMTP_HOST"],
                Port = int.Parse(Configuration["SMTP_PORT"]),
                Credentials = new NetworkCredential(Configuration["SMTP_USER"], Configuration["SMTP_PASSWORD"]),
                EnableSsl = true
            };
        }

        private static HttpClient CreateHttpClient()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            return new HttpClient
            {
                BaseAddress = new Uri(Configuration["FILES_URI"])
            };
        }
    }
}
