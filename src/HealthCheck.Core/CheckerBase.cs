using System;
using System.Threading.Tasks;
using HealthCheck.Core.Extensions;
using HealthCheck.Core.Results;

namespace HealthCheck.Core
{
    public abstract class CheckerBase : IChecker
    {
        public string Name { get; set; }
        public string SectionName { get; set; }
        public virtual TimeSpan? Timeout { private get; set; }

        protected CheckerBase(String name, String sectionName) : this(name)
        {
            SectionName = sectionName;
        }

        protected CheckerBase(String name)
        {
            Name = name;
        }

        protected ICheckResult CreateResult(bool passed, string output)
        {
            return new CheckResult
            {
                CheckerName = Name,
                SectionName = SectionName,
                Passed = passed,
                Output = output
            };
        }

        protected Task<ICheckResult> CreateTaskResult(bool passed, string output)
        {
            return Task.FromResult(CreateResult(passed, output));
        }

        public async Task<ICheckResult> Check()
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

        protected abstract Task<ICheckResult> CheckCore();
    }
}
