using System;
using System.Threading.Tasks;
using HealthCheck.Core;
using HealthCheck.Windows.Metrics;

namespace HealthCheck.Windows.Checkers
{
    public class CpuUsageIsOk : CheckerBase
    {
        private readonly ICpuUsage _cpuUsage;
        private readonly Options _options; 

        public override string Name
        {
            get { return "CPU usage is OK"; }
        }

        public CpuUsageIsOk(
            ICpuUsage cpuUsage, 
            Options options = null)
        {
            _cpuUsage = cpuUsage;
            _options = options ?? new Options();
        }

        protected override async Task<CheckResult> CheckCore()
        {
            var currentCpuUsage = await _cpuUsage.Read().ConfigureAwait(false);
            return CreateResult(
                currentCpuUsage < _options.CpuUsageWarningThresholdInPercent, 
                string.Format("Currently using {0:F1} % of total processor time.", currentCpuUsage));
        }

        public class Options
        {
            public float CpuUsageWarningThresholdInPercent { get; set; }

            public Options()
            {
                CpuUsageWarningThresholdInPercent = 90;
            }
        }
    }
}