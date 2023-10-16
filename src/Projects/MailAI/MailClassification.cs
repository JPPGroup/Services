namespace Jpp.Projects.MailAI
{
    public class MailClassification
    {
        public string Id { get; set; }

        public string Sender { get; set; }
        public string Reciever { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
        public string Classification { get; set; }

        public bool HasAttachments { get; set; }

    }
}
