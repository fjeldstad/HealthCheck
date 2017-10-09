using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public abstract class HealthCheckBase
    {
        protected readonly IEnumerable<IChecker> Checkers;

        protected HealthCheckBase(IEnumerable<IChecker> checkers)
        {
            Checkers = checkers;
        }

        public abstract Task<HealthCheckResult> Run();
    }
}
