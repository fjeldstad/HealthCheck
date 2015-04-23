using System;
using System.Threading.Tasks;
using HealthCheck.Core;
using HealthCheck.Redis.Metrics;

namespace HealthCheck.Redis.Checkers
{
    public class RedisHasFreeMemory : CheckerBase
    {
        private readonly IRedisMemoryUsage _redisMemoryUsage;
        private readonly IRedisMaxMemory _redisMaxMemory;
        private readonly Options _options;

        public override string Name
        {
            get { return "Redis has free memory"; }
        }

        public RedisHasFreeMemory(
            IRedisMemoryUsage redisMemoryUsage, 
            IRedisMaxMemory redisMaxMemory, 
            Options options = null)
        {
            _redisMemoryUsage = redisMemoryUsage;
            _redisMaxMemory = redisMaxMemory;
            _options = options ?? new Options();
        }

        protected override async Task<CheckResult> CheckCore()
        {
            var maxBytes = await _redisMaxMemory.Read().ConfigureAwait(false);
            var usedBytes = await _redisMemoryUsage.Read().ConfigureAwait(false);
            if (!maxBytes.HasValue || maxBytes.Value == 0)
            {
                return CreateResult(
                    true,
                    string.Format(
                        "Currently using {0:F1} MB, no hard upper limit configured.",
                        usedBytes / (double)(1024 * 1024)));
            }
            var usage = usedBytes / (double)maxBytes;
            var freeMegabytes = (maxBytes - usedBytes) / (double)(1024 * 1024);
            return CreateResult(
                maxBytes - usedBytes > _options.RedisFreeMemoryWarningThresholdInBytes,
                string.Format(
                    "{0:P0} of the reserved memory is used ({1:F1} MB free).",
                    usage,
                    freeMegabytes));
        }

        public class Options
        {
            public long RedisFreeMemoryWarningThresholdInBytes { get; set; }

            public Options()
            {
                RedisFreeMemoryWarningThresholdInBytes = 50L * 1024L * 1024L; // 50 MB
            }
        }
    }
}