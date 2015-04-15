using System.Collections.Generic;
using System.Linq;

namespace HealthCheck.Core
{
    public class HealthCheckResult
    {
        private readonly CheckResult[] _results;
        private readonly HealthCheckStatus _status;

        public CheckResult[] Results { get { return _results; } }
        public HealthCheckStatus Status { get { return _status; } }

        public HealthCheckResult(IEnumerable<CheckResult> results)
        {
            _results = results.OrderBy(x => x.Checker).ToArray();
            _status = _results.All(x => x.Passed) ? HealthCheckStatus.Success : HealthCheckStatus.Failure;
        }
    }

    public enum HealthCheckStatus
    {
        Success,
        Failure
    }
}
