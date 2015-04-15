using System.Collections.Generic;
using System.Linq;

namespace HealthCheck.Core
{
    public class HealthCheckResult
    {
        private readonly CheckResult[] _results;
        private readonly HealthCheckResultStatus _status;

        public CheckResult[] Results { get { return _results; } }
        public HealthCheckResultStatus Status { get { return _status; } }

        public HealthCheckResult(IEnumerable<CheckResult> results)
        {
            _results = results.OrderBy(x => x.Checker).ToArray();
            _status = _results.All(x => x.Passed) ? HealthCheckResultStatus.Success : HealthCheckResultStatus.Failure;
        }
    }

    public enum HealthCheckResultStatus
    {
        Success,
        Failure
    }
}
