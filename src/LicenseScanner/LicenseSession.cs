namespace LicenseScanner
{
    public class LicenseSession
    {
        public Guid Id { get; set; }
        public LicenseType Type { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Username { get; set; }
    }
}
