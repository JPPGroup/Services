using System.Collections.Concurrent;

namespace CustodianWorker.Analyzers
{
    public abstract class Analyzer
    {
        public IReadOnlyList<Violation> Violations
        {
            get { return _violations.ToList(); }
        }

        protected ConcurrentBag<Violation> _violations;

        public Analyzer()
        {
            _violations = new ConcurrentBag<Violation>();
        }
    }
}
