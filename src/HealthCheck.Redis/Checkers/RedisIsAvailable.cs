﻿using System;
using System.Threading.Tasks;
using HealthCheck.Core;
using HealthCheck.Core.Results;
using HealthCheck.Redis.Metrics;

namespace HealthCheck.Redis.Checkers
{
    public class RedisIsAvailable : CheckerBase
    {
        private readonly IRedisVersion _redisVersion;
        private readonly IRedisUptime _redisUptime;

        public RedisIsAvailable(
            IRedisVersion redisVersion, 
            IRedisUptime redisUptime) : base("Redis is available")
        {
            _redisVersion = redisVersion;
            _redisUptime = redisUptime;
        }

        protected override async Task<ICheckResult> CheckCore()
        {
            var redisVersion = await _redisVersion.Read().ConfigureAwait(false);
            var redisUptime = await _redisUptime.Read().ConfigureAwait(false);
            return CreateResult(
                true, 
                string.Format("Version {0}, uptime {1:%d} days {1:%h} hours {1:%m} minutes.", redisVersion, redisUptime));
        }
    }
}