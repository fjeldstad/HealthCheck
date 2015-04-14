using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using HealthCheck.Core;

namespace HealthCheck.Windows.Metrics
{
    public interface ICpuUsage : IMetric<float>
    {
    }

    public class CpuUsage : ICpuUsage, IDisposable
    {
        private static readonly object Token = new object();
        private readonly PerformanceCounter _performanceCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private readonly Timer _timer = new Timer();
        private float _lastCpuUsageValue;

        public CpuUsage()
        {
            _performanceCounter.NextValue();
            _timer.AutoReset = true;
            _timer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
            _timer.Elapsed += (sender, args) =>
            {
                lock (Token)
                {
                    _lastCpuUsageValue = _performanceCounter.NextValue();
                }
            };
            _timer.Start();
        }

        public Task<float> Read()
        {
            lock (Token)
            {
                return Task.FromResult(_lastCpuUsageValue);
            }
        }

        public void Dispose()
        {
            _timer.Stop();
            _timer.Dispose();
        }
    }

}
