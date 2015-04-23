using System;
using System.Threading.Tasks;
using HealthCheck.Core;
using HealthCheck.Redis.Metrics;

namespace HealthCheck.Redis.Checkers
{
    public class RedisIsAvailable : CheckerBase
    {
        private readonly IRedisVersion _redisVersion;
        private readonly IRedisUptime _redisUptime;

        public override string Name
        {
            get { return "Redis is available"; }
        }

        public RedisIsAvailable(
            IRedisVersion redisVersion, 
            IRedisUptime redisUptime)
        {
            _redisVersion = redisVersion;
            _redisUptime = redisUptime;
        }

        protected override async Task<CheckResult> CheckCore()
        {
            var redisVersion = await _redisVersion.Read().ConfigureAwait(false);
            var redisUptime = await _redisUptime.Read().ConfigureAwait(false);
            return CreateResult(
                true, 
                string.Format("Version {0}, uptime {1:%d} days {1:%h} hours {1:%m} minutes.", redisVersion, redisUptime));
        }
    }
}