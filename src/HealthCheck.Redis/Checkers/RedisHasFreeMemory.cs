using System.Threading.Tasks;
using HealthCheck.Core;
using HealthCheck.Redis.Metrics;

namespace HealthCheck.Redis.Checkers
{
    public class RedisHasFreeMemory : CheckerBase
    {
        private readonly IRedisMemoryUsage _redisMemoryUsage;
        private readonly Options _options;

        public override string Name
        {
            get { return "Redis has free memory"; }
        }

        public RedisHasFreeMemory(IRedisMemoryUsage redisMemoryUsage, Options options)
        {
            _redisMemoryUsage = redisMemoryUsage;
            _options = options;
        }

        protected override async Task<CheckResult> CheckCore()
        {
            var usedBytes = await _redisMemoryUsage.Read().ConfigureAwait(false);
            var usage = usedBytes / (double)_options.RedisConfiguredMaxMemoryUsageInBytes;
            var freeMegabytes = (_options.RedisConfiguredMaxMemoryUsageInBytes - usedBytes) / (double)(1024 * 1024);
            return CreateResult(
                usedBytes < _options.RedisMemoryUsageWarningThresholdInBytes,
                string.Format(
                    "{0:P0} of the reserved memory is used ({1:F1} MB free).",
                    usage,
                    freeMegabytes));
        }

        public class Options
        {
            public long RedisConfiguredMaxMemoryUsageInBytes { get; set; }
            public long RedisMemoryUsageWarningThresholdInBytes { get; set; }
        }
    }
}