using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCheck.Redis.Checkers;
using HealthCheck.Redis.Metrics;
using Moq;
using NUnit.Framework;

namespace HealthCheck.Redis.Tests.Checkers
{
    [TestFixture]
    public class RedisHasFreeMemoryTests
    {
        [Test]
        public async Task CheckPassesWhenMemoryUsageIsBelowThreshold()
        {
            // Arrange
            var options = new RedisHasFreeMemory.Options
            {
                RedisMemoryUsageWarningThresholdInBytes = 1024
            };
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            redisMemoryUsageMock.Setup(x => x.Read()).ReturnsAsync(options.RedisMemoryUsageWarningThresholdInBytes - 1);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, options);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.True);
        }

        [Test]
        public async Task CheckFailsWhenMemoryUsageIsAboveThreshold()
        {
            // Arrange
            var options = new RedisHasFreeMemory.Options
            {
                RedisMemoryUsageWarningThresholdInBytes = 1024
            };
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            redisMemoryUsageMock.Setup(x => x.Read()).ReturnsAsync(options.RedisMemoryUsageWarningThresholdInBytes + 1);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, options);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
        }

        [Test]
        public async Task CheckFailsWhenRedisMemoryUsageThrows()
        {
            // Arrange
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            var exception = new Exception("error message");
            redisMemoryUsageMock.Setup(x => x.Read()).ThrowsAsync(exception);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, new RedisHasFreeMemory.Options());

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }
    }
}
