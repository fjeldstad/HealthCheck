﻿using System;
using System.Threading.Tasks;
using HealthCheck.Core.Extensions;

namespace HealthCheck.Core
{
    public abstract class CheckerBase : IChecker
    {
        public string Name { get; set; }
        public string SectionName { get; set; }
        public virtual bool PreserveContext => false;
        public virtual TimeSpan? Timeout { get; set; }

        protected CheckerBase(string name, string sectionName) : this(name)
        {
            SectionName = sectionName;
        }

        protected CheckerBase(string name)
        {
            Name = name;
        }

        protected CheckResult CreateResult(bool passed, string output)
        {
            return new CheckResult
            {
                Checker = Name,
                SectionName = SectionName,
                Passed = passed,
                Output = output
            };
        }

        protected Task<CheckResult> CreateTaskResult(bool passed, string output)
        {
            return Task.FromResult(CreateResult(passed, output));
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
