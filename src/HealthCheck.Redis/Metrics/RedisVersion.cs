using System;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core;
using StackExchange.Redis;

namespace HealthCheck.Redis.Metrics
{
    public interface IRedisVersion : IMetric<string>
    {
    }

    public class RedisVersion : IRedisVersion
    {
        private readonly Func<IServer> _redisServer;

        public RedisVersion(Func<IServer> redisServer)
        {
            _redisServer = redisServer;
        }

        public async Task<string> Read()
        {
            var info = (await _redisServer().InfoAsync("Server").ConfigureAwait(false)).First();
            return info.First(x => x.Key.Equals("redis_version")).Value;
        }
    }
}
