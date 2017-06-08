using System;
using System.Collections.Generic;
using System.Linq;

namespace HealthCheck.Core.Results
{
    public class HealthCheckResult
    {
        private readonly ICheckResult[] _results;
        private readonly bool _passed;

        public ICheckResult[] Results => _results;
        public bool Passed => _passed;
        public string Output => _passed ? "SUCCESS" : "FAILURE";

        public HealthCheckResult(IEnumerable<ICheckResult> results)
        {
            _results = results.OrderBy(x => x.CheckerName).ToArray();
            _passed = _results.Any() && _results.All(x => x.Passed);
        }
    }
}
