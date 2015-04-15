using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCheck.Core;
using StackExchange.Redis;

namespace HealthCheck.Redis.Metrics
{
    public interface IRedisMaxMemory : IMetric<long?>
    {
    }

    public class RedisMaxMemory : IRedisMaxMemory
    {
        private readonly Func<IServer> _redisServer;

        public RedisMaxMemory(Func<IServer> redisServer)
        {
            _redisServer = redisServer;
        }

        public async Task<long?> Read()
        {
            string maxMemory = (await _redisServer().ConfigGetAsync("maxmemory").ConfigureAwait(false))
                .Select(x => x.Value)
                .FirstOrDefault();
            if (string.IsNullOrEmpty(maxMemory))
            {
                return null;
            }
            long maxMemoryBytes;
            return long.TryParse(maxMemory, out maxMemoryBytes) ?
                maxMemoryBytes :
                (long?)null;
        }
    }
}
