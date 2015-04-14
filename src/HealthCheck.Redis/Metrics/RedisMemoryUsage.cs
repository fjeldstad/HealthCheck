using System;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core;
using StackExchange.Redis;

namespace HealthCheck.Redis.Metrics
{
    public interface IRedisMemoryUsage : IMetric<long>
    {
    }

    public class RedisMemoryUsage : IRedisMemoryUsage
    {
        private readonly Func<IServer> _redisServer;

        public RedisMemoryUsage(Func<IServer> redisServer)
        {
            _redisServer = redisServer;
        }

        public async Task<long> Read()
        {
            var info = (await _redisServer().InfoAsync("Memory").ConfigureAwait(false)).First();
            return long.Parse(info.First(x => x.Key.Equals("used_memory")).Value);
        }
    }
}
