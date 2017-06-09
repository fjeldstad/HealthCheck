using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core.Results;

namespace HealthCheck.Core
{
    public abstract class HealthCheckBase : IHealthCheck
    {
        protected readonly IEnumerable<IChecker> Checkers;

        protected HealthCheckBase(IEnumerable<IChecker> checkers)
        {
            Checkers = checkers;
        }

        public abstract Task<HealthCheckResult> Run();
    }
}
