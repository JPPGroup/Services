using Jpp.MessageBroker;
using Jpp.MessageBroker.Generics;
using Jpp.MessageBroker.Mapping;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Jpp.MappingReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            /*CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

#if !DEBUG
            Task background = Task.Run(async () =>
            {
                var capturedToken = token;

                Console.WriteLine($"Attempting to create channel.");
                using (MappingReportGeneratorChannel channel = new MappingReportGeneratorChannel(true))
                {
                    Console.WriteLine($"Channel created");

                    TileProvider provider = new TileProvider();

                    var client = new HttpClient { BaseAddress = new Uri("http://files") };
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
                    client.DefaultRequestHeaders.Accept.Clear();

                    while (!capturedToken.IsCancellationRequested)
                    {
                        Console.WriteLine($"\nAwaiting message.");
                        var workMessage = await channel.ReceiveMessageAsync();
                        Console.WriteLine($"Message received");

                        //MappingReport mappingReport = new MappingReport(provider, "Brixworth Office", "Bob the Builder", Guid.NewGuid().ToString().Remove(13), new WGS84(52.332510, -0.897930));
                        //MappingReport mappingReport = new MappingReport(provider, "Trafalgar Square", "Bob the Builder", Guid.NewGuid().ToString().Remove(13), new WGS84(51.5074, -0.1278));
                        //MappingReport mappingReport = new MappingReport(provider, "Data Check", "Bob the Builder", Guid.NewGuid().ToString().Remove(13), new WGS84(52.329390, -0.601260));
                        string ReferenceNumber = Guid.NewGuid().ToString().Remove(13);
                        using (MappingReport mappingReport = new MappingReport(provider, workMessage.Project, workMessage.Client, ReferenceNumber, new WGS84(workMessage.Latitude, workMessage.Longitude)))
                        {
                            mappingReport.AddDrawing(DrawingType.OSLarge);
                            mappingReport.AddDrawing(DrawingType.OS);
                            mappingReport.AddDrawing(DrawingType.Radon);
                            mappingReport.AddDrawing(DrawingType.FloodZone2);
                            mappingReport.Finalise();
                        }

                        Console.WriteLine($"Request processed");

                        using (var request = new HttpRequestMessage(HttpMethod.Post, $"api/files"))
                        {
                            using (var content = new MultipartFormDataContent())
                            {
                                var fileName = $"{ReferenceNumber}.pdf";
                                using (var byteStream = File.OpenRead(fileName))
                                {
                                    content.Add(new StreamContent(byteStream), "file", fileName);                              
                                    request.Content = content;

                                    var response = await client.SendAsync(request);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine($"File ID: {response.Content.ReadAsStringAsync().Result}");
                                        Console.WriteLine($"File saved");
                                        channel.RequestComplete();
                                        Console.WriteLine($"Request complete.\n");
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Save failed with error {response.StatusCode} - {response.ReasonPhrase}");
                                        channel.RequestFailed();
                                    }
                                }
                            }
                        }

                        //return new Attachment(contentStream, new ContentType(content.Content.Headers.ContentType.MediaType));

                        File.Delete($"{ReferenceNumber}.pdf");
                        Console.WriteLine($"Temp file removed");                        
                    }
                }
                Console.WriteLine("Shutdown work thread.");
            }, token);
#endif

            // Handle Control+C or Control+Break
            Console.CancelKeyPress += (o, e) =>
            {
                Console.WriteLine("Termination requested.");

                // Allow the manin thread to continue and exit...
                tokenSource.Cancel();
                e.Cancel = true;
            };

#if DEBUG
            TileProvider provider = new TileProvider();

            //MappingReport mappingReport = new MappingReport(provider, "Brixworth Office", "Bob the Builder", Guid.NewGuid().ToString().Remove(13), new WGS84(52.332510, -0.897930));
            //MappingReport mappingReport = new MappingReport(provider, "Trafalgar Square", "Bob the Builder", Guid.NewGuid().ToString().Remove(13), new WGS84(51.5074, -0.1278));
            for (int i = 0; i < 1; i++)
            {
                //using (MappingReport mappingReport = new MappingReport(provider, "Data Check", "Bob the Builder", Guid.NewGuid().ToString().Remove(13), new WGS84(52.329390, -0.601260)))
                using (MappingReport mappingReport = new MappingReport(provider, "Test Project", "Mr. A Client", Guid.NewGuid().ToString().Remove(13), new WGS84(52.351550, 0.149740)))
                {
                    mappingReport.AddCover();
                    mappingReport.AddContents();
                    mappingReport.AddDrawing(DrawingType.OSLarge);
                    mappingReport.AddDrawing(DrawingType.SatelliteLarge);
                    mappingReport.AddDrawing(DrawingType.HistoricLarge);
                    mappingReport.AddDrawing(DrawingType.OS);
                    mappingReport.AddDrawing(DrawingType.Satellite);
                    mappingReport.AddDrawing(DrawingType.Historic);
                    mappingReport.AddDrawing(DrawingType.OSSmall);
                    mappingReport.AddDrawing(DrawingType.SatelliteSmall);
                    mappingReport.AddDrawing(DrawingType.HistoricSmall);
                    mappingReport.AddDrawing(DrawingType.Radon);
                    mappingReport.AddDrawing(DrawingType.SUperficialGeo);
                    mappingReport.AddDrawing(DrawingType.FloodZone2);
                    mappingReport.AddDrawing(DrawingType.FloodZone3);
                    mappingReport.Finalise();
                }

                GC.Collect();
            }
            Console.WriteLine("Done.");
            Console.ReadLine();
#endif

// Wait
#if !DEBUG
            background.Wait();
#endif
            Console.WriteLine("Terminated.");     */       
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<IReceiveChannel<GenerateRequestMessage>, GenerateRequestReceiveChannel>();
                });
    }
}

