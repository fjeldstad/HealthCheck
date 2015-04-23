using System;
using System.Threading.Tasks;
using HealthCheck.Core;
using HealthCheck.Windows.Metrics;

namespace HealthCheck.Windows.Checkers
{
    public class SystemDriveHasFreeSpace : CheckerBase
    {
        private readonly IAvailableSystemDriveSpace _availableSystemDriveSpace;
        private readonly Options _options;

        public override string Name
        {
            get { return "System drive has free space"; }
        }

        public SystemDriveHasFreeSpace(IAvailableSystemDriveSpace availableSystemDriveSpace, Options options = null)
        {
            _availableSystemDriveSpace = availableSystemDriveSpace;
            _options = options ?? new Options();
        }

        protected override async Task<CheckResult> CheckCore()
        {
            var freeBytes = await _availableSystemDriveSpace.Read().ConfigureAwait(false);
            return CreateResult(
                freeBytes >= _options.SystemDriveAvailableFreeSpaceWarningThresholdInBytes,
                string.Format("{0:F1} GB available.", freeBytes / (double)(1024 * 1024 * 1024)));
        }

        public class Options
        {
            public long SystemDriveAvailableFreeSpaceWarningThresholdInBytes { get; set; }

            public Options()
            {
                SystemDriveAvailableFreeSpaceWarningThresholdInBytes = 5L * 1024L * 1024L * 1024L; // 5 GB
            }
        }
    }
}