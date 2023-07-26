using CustodianWorker.Analyzers;
using Microsoft.Exchange.WebServices.Data;
using Task = System.Threading.Tasks.Task;

namespace CustodianWorker
{
    public class MailWorker : BackgroundService
    {
        private readonly ILogger<MailWorker> _logger;
        private readonly ExchangeService _service;
        private readonly FolderAnalyzer _folderAnalyzer;

        public MailWorker(ILogger<MailWorker> logger, ILogger<FolderAnalyzer> analyzerLogger)
        {
            _logger = logger;
            _service = new ExchangeService();
            _folderAnalyzer = new FolderAnalyzer(analyzerLogger);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Mail Worker running at: {time}", DateTimeOffset.Now);
            /*while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }*/

            _service.Credentials = new WebCredentials("michael.liddiard@jppuk.net", "London4");
            _service.AutodiscoverUrl("michael.liddiard@jppuk.net");

            /*PropertySet propSet = new PropertySet(BasePropertySet.FirstClassProperties);
            Folder rootfolder = Folder.Bind(service, WellKnownFolderName.Inbox, propSet);*/

            /*FolderId SharedMailbox = new FolderId(WellKnownFolderName.Inbox, "JPP_20000@jppuk.net");
            ItemView itemView = new ItemView(1000);
            service.FindItems(SharedMailbox, itemView);*/

            FolderId SharedMailbox = new FolderId(WellKnownFolderName.MsgFolderRoot, "JPP_20000@jppuk.net");
            await RecurseSearchFolder(_service, SharedMailbox);
        }

        private async Task RecurseSearchFolder(ExchangeService service, FolderId folderId)
        {
            FolderView view = new FolderView(1000);
            // Create an extended property definition for the PR_ATTR_HIDDEN property,
            // so that your results will indicate whether the folder is a hidden folder.
            ExtendedPropertyDefinition isHiddenProp = new ExtendedPropertyDefinition(0x10f4, MapiPropertyType.Boolean);
            // As a best practice, limit the properties returned to only those required.
            // In this case, return the folder ID, DisplayName, and the value of the isHiddenProp
            // extended property.
            view.PropertySet = new PropertySet(BasePropertySet.IdOnly, FolderSchema.DisplayName, isHiddenProp, FolderSchema.ChildFolderCount);
            // Indicate a Traversal value of Deep, so that all subfolders are retrieved.
            //view.Traversal = FolderTraversal.Deep;

            FindFoldersResults findFolderResults = service.FindFolders(folderId, view);
            //Does pagination cause an issue?

            var initFunc = new Func<ExchangeService>(() =>
            {
                ExchangeService innerService = new ExchangeService();
                innerService.Credentials = new WebCredentials("michael.liddiard@jppuk.net", "London4");
                //service.AutodiscoverUrl("michael.liddiard@jppuk.net");
                innerService.Url = _service.Url;
                return innerService;
            });

            var workerBody = new Func<Folder, ParallelLoopState, ExchangeService, ExchangeService>((folder, state, innerService) =>
            {
                if (folder.ChildFolderCount > 0)
                {
                    _logger.LogTrace("Scanning {DisplayName}", folder.DisplayName);
                    RecurseSearchFolder(innerService, folder.Id);
                }
                else
                {
                    _logger.LogTrace("{DisplayName} has no subfolders", folder.DisplayName);
                    _folderAnalyzer.AnalyzeFolder(innerService, folder);
                }

                return innerService;
            });

            //Parallel.ForEach<Folder, ExchangeService>(findFolderResults.Folders, new ParallelOptions { MaxDegreeOfParallelism = 2 }, initFunc, workerBody, exchangeService => { return; });
            Parallel.ForEach<Folder, ExchangeService>(findFolderResults.Folders, initFunc, workerBody, exchangeService => { return; });
        }
    }
}