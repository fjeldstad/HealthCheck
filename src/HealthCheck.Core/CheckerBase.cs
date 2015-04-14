using System;
using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public abstract class CheckerBase : IChecker
    {
        public abstract string Name { get; }

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
