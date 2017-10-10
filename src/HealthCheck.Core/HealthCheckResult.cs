using System.Collections.Generic;
using System.Linq;

namespace HealthCheck.Core
{
    public class HealthCheckResult
    {
        private readonly CheckResult[] _results;
        private readonly bool _passed;

        public CheckResult[] Results => _results;
        public bool Passed => _passed;
        public string Output => _passed ? "success" : "failure";

        public HealthCheckResult(IEnumerable<CheckResult> results)
        {
            _results = results.OrderBy(x => x.Checker).ToArray();
            _passed = _results.Any() && _results.All(x => x.Passed);
        }
    }
}
