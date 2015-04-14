using System;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core;
using StackExchange.Redis;

namespace HealthCheck.Redis.Metrics
{
    public interface IRedisUptime : IMetric<TimeSpan>
    {
    }

    public class RedisUptime : IRedisUptime
    {
        private readonly Func<IServer> _redisServer;

        public RedisUptime(Func<IServer> redisServer)
        {
            _redisServer = redisServer;
        }

        public async Task<TimeSpan> Read()
        {
            var info = (await _redisServer().InfoAsync("Server").ConfigureAwait(false)).First();
            return TimeSpan.FromSeconds(int.Parse(info.First(x => x.Key.Equals("uptime_in_seconds")).Value));
        }
    }
}
