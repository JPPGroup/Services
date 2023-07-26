using Microsoft.Exchange.WebServices.Data;

namespace CustodianWorker.Analyzers
{
    public class FolderAnalyzer : Analyzer
    {
        private ExtendedPropertyDefinition PidTagEntryId;
        private ExtendedPropertyDefinition MailInternetId;

        private readonly ILogger<FolderAnalyzer> _logger;

        public FolderAnalyzer(ILogger<FolderAnalyzer> logger)
        {
            _logger = logger;
            PidTagEntryId = new ExtendedPropertyDefinition(0x300B, MapiPropertyType.Binary);
            MailInternetId = new ExtendedPropertyDefinition(0x1035, MapiPropertyType.String);
        }

        public void AnalyzeFolder(ExchangeService service, Folder folder)
        {
            ItemView itemView = new ItemView(int.MaxValue);

            itemView.PropertySet = new PropertySet(BasePropertySet.IdOnly, PidTagEntryId, MailInternetId);
            FindItemsResults<Item> searchResults = service.FindItems(folder.Id, itemView);
            int dupeCount = DuplicateCheck(searchResults);

            if (dupeCount > 0)
            {
                var violation = new DuplicateViolation()
                {
                    DuplicateCount = dupeCount,
                    FolderId = folder.Id.ToString(),
                    FolderName = folder.DisplayName
                };
                _violations.Add(violation);
                _logger.LogInformation("Duplicate Violation - {ProjectCode} contains {DuplicateCount} duplicates", violation.FolderName, violation.DuplicateCount);
            }
        }

        private int DuplicateCheck(FindItemsResults<Item> items)
        {
            int dupeCount = 0;

            //Review for pagaing
            for (int i = 0; i < items.Items.Count; i++)
            {
                string messageId = GetEntryId(items.Items[i]);


                for (int y = i + 1; y < items.Items.Count; y++)
                {
                    string testMessageId = GetEntryId(items.Items[y]);

                    if (messageId.Equals(testMessageId))
                    {
                        dupeCount++;
                    }
                }
            }

            return dupeCount;
        }

        private string GetEntryId(Item item)
        {
            /*var originalId = new AlternateId(IdFormat.EwsId, item.Id.ToString(), "JPP_20000@jppuk.net", false);
            return ((AlternateId)_service.ConvertId(originalId, IdFormat.HexEntryId)).UniqueId;*/
            /*byte[] bytes = null;
            if (!item.TryGetProperty(PidTagEntryId, out bytes))
                throw new InvalidOperationException();

            var HexSearchKey = BitConverter.ToString(bytes).Replace("-", "");
            return HexSearchKey;*/

            string internetId = string.Empty;
            if (!item.TryGetProperty(MailInternetId, out internetId))
                throw new InvalidOperationException();

            return internetId;
        }
    }

    public class DuplicateViolation : Violation
    {
        public DuplicateViolation()
        {
            Severity = Severity.Warning;
        }

        public int DuplicateCount { get; set; }

        public string FolderId { get; set; }
        public string FolderName { get; set; }
    }
}
