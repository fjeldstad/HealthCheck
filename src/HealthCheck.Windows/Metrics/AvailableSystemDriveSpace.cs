using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core;

namespace HealthCheck.Windows.Metrics
{
    public interface IAvailableSystemDriveSpace : IMetric<long>
    {
    }

    public class AvailableSystemDriveSpace : IAvailableSystemDriveSpace
    {
        public Task<long> Read()
        {
            var systemDriveName = Path.GetPathRoot(Environment.GetFolderPath(Environment.SpecialFolder.System));
            return Task.FromResult(
                DriveInfo.GetDrives()
                         .Single(x => x.IsReady && x.Name.Equals(systemDriveName, StringComparison.OrdinalIgnoreCase))
                         .AvailableFreeSpace);
        }
    }
}
