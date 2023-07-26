namespace CustodianWorker.Analyzers
{
    public abstract class Violation
    {
        public Severity Severity { get; set; }
    }

    public enum Severity
    {
        Error,
        Warning,
        Information
    }
}
