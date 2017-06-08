using System;
using System.Collections.Generic;

namespace HealthCheck.Core.Configuration
{
    public class ConfiguratorBase : IHealthCheckConfigurator
    {
        public Func<IEnumerable<IChecker>> GetCheckers { get; set; }

        public IHealthCheckConfigurator ConfigureCheckers(Func<IEnumerable<IChecker>> getCheckers)
        {
            if (GetCheckers == null)
            {
                GetCheckers = getCheckers;
            }
            else
            {
                GetCheckers = GetCheckers + getCheckers;
            }
            return this;
        }
    }
}