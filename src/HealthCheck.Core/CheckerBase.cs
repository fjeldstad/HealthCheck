using System;
using System.Threading.Tasks;
using HealthCheck.Core.Helpers;

namespace HealthCheck.Core
{
    public abstract class CheckerBase : IChecker
    {
        private TimeSpan? _timeout = TimeSpan.FromSeconds(5);

        public abstract string Name { get; }
        public TimeSpan? Timeout { get { return _timeout; } set { _timeout = value; } }

        protected CheckResult CreateResult(bool passed, string output)
        {
            return new CheckResult
            {
                Checker = Name,
                Passed = passed,
                Output = output
            };
        }

        public async Task<CheckResult> Check()
        {
            try
            {
                if (Timeout.HasValue && Timeout.Value > TimeSpan.Zero)
                {
                    return await CheckCore().TimeoutAfter(Timeout.Value).ConfigureAwait(false);
                }
                return await CheckCore().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                return CreateResult(false, ex.Message);
            }
        }

        protected abstract Task<CheckResult> CheckCore();
    }
}
