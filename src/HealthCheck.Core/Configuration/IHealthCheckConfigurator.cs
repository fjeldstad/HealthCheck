using System;
using System.Collections.Generic;

namespace HealthCheck.Core.Configuration
{
    public interface IHealthCheckConfigurator
    {
        IHealthCheckConfigurator ConfigureCheckers(Func<IEnumerable<IChecker>> getChechers);
    }
}