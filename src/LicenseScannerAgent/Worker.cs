using System.Security.AccessControl;
using System.Security.Principal;

namespace LicenseScannerAgent
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private string _licensePath = @"\\fileserver.cedarbarn.local\companydata\consulting\CADS_RC";
        private string _licenseFilter = "*.net";

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ForceSync();

            using var watcher = new FileSystemWatcher(@"\\fileserver.cedarbarn.local\companydata\consulting\CADS_RC");
            watcher.Filter = _licenseFilter;
            watcher.IncludeSubdirectories = false;
            watcher.EnableRaisingEvents = true;

            watcher.Changed += FileChanged;
            watcher.Created += FileChanged;
            watcher.Deleted += FileChanged;

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        private void FileChanged(object sender, FileSystemEventArgs e)
        {
            switch (e.ChangeType)
            {
                case WatcherChangeTypes.Deleted:
                    Console.WriteLine($"{e.ChangeType} at {e.Name}");
                    break;

                case WatcherChangeTypes.Created:
                    var fileSecurity = new FileSecurity(e.FullPath, AccessControlSections.Owner);
                    string owner = fileSecurity.GetOwner(typeof(NTAccount)).ToString();
                    Console.WriteLine($"{e.ChangeType} at {e.Name} - owner now {owner}");
                    break;
            }
        }

        private void ForceSync()
        {
            foreach (string file in Directory.GetFiles(_licensePath, _licenseFilter))
            {
                var fileSecurity = new FileSecurity(file, AccessControlSections.Owner);
                string owner = fileSecurity.GetOwner(typeof(NTAccount)).ToString();
                Console.WriteLine($"{file} - owner is {owner}");
            }
        }
    }
}