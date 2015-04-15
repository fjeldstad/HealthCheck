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
        public async Task CheckPassesWhenFreeMemoryIsAboveThreshold()
        {
            // Arrange
            var options = new RedisHasFreeMemory.Options
            {
                RedisFreeMemoryWarningThresholdInBytes = 1024
            };
            long maxMemory = options.RedisFreeMemoryWarningThresholdInBytes * 10;
            long usedMemory = maxMemory - options.RedisFreeMemoryWarningThresholdInBytes - 1;
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            redisMemoryUsageMock
                .Setup(x => x.Read())
                .ReturnsAsync(usedMemory);
            var redisMaxMemoryMock = new Mock<IRedisMaxMemory>();
            redisMaxMemoryMock
                .Setup(x => x.Read())
                .ReturnsAsync(maxMemory);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, redisMaxMemoryMock.Object, options);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.True);
        }

        [Test]
        public async Task CheckFailsWhenFreeMemoryIsBelowThreshold()
        {
            // Arrange
            var options = new RedisHasFreeMemory.Options
            {
                RedisFreeMemoryWarningThresholdInBytes = 1024
            };
            long maxMemory = options.RedisFreeMemoryWarningThresholdInBytes * 10;
            long usedMemory = maxMemory - options.RedisFreeMemoryWarningThresholdInBytes + 1;
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            redisMemoryUsageMock
                .Setup(x => x.Read())
                .ReturnsAsync(usedMemory);
            var redisMaxMemoryMock = new Mock<IRedisMaxMemory>();
            redisMaxMemoryMock
                .Setup(x => x.Read())
                .ReturnsAsync(maxMemory);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, redisMaxMemoryMock.Object, options);

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
            var redisMaxMemoryMock = new Mock<IRedisMaxMemory>();
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, redisMaxMemoryMock.Object, new RedisHasFreeMemory.Options());

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }

        [Test]
        public async Task CheckFailsWhenRedisMaxMemoryThrows()
        {
            // Arrange
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            var exception = new Exception("error message");
            var redisMaxMemoryMock = new Mock<IRedisMaxMemory>();
            redisMaxMemoryMock.Setup(x => x.Read()).ThrowsAsync(exception);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, redisMaxMemoryMock.Object, new RedisHasFreeMemory.Options());

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }

        [Test]
        public async Task CheckPassesWhenThereIsNoUpperLimitOnMemoryUsage()
        {
            // Arrange
            var redisMaxMemoryMock = new Mock<IRedisMaxMemory>();
            redisMaxMemoryMock.Setup(x => x.Read()).ReturnsAsync(null);
            var redisMemoryUsageMock = new Mock<IRedisMemoryUsage>();
            redisMemoryUsageMock.Setup(x => x.Read()).ReturnsAsync(1024);
            var check = new RedisHasFreeMemory(redisMemoryUsageMock.Object, redisMaxMemoryMock.Object, new RedisHasFreeMemory.Options());

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.True);
        }
    }
}
