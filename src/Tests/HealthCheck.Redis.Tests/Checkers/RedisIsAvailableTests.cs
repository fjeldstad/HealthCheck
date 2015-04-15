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
    public class RedisIsAvailableTests
    {
        [Test]
        public async Task CheckFailsWhenRedisVersionThrows()
        {
            // Arrange
            var redisVersionMock = new Mock<IRedisVersion>();
            var exception = new Exception("error message");
            redisVersionMock.Setup(x => x.Read()).ThrowsAsync(exception);
            var redisUptimeMock = new Mock<IRedisUptime>();
            redisUptimeMock.Setup(x => x.Read()).ReturnsAsync(TimeSpan.FromDays(1));
            var check = new RedisIsAvailable(redisVersionMock.Object, redisUptimeMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }

        [Test]
        public async Task CheckFailsWhenRedisUptimeThrows()
        {
            // Arrange
            var redisVersionMock = new Mock<IRedisVersion>();
            redisVersionMock.Setup(x => x.Read()).ReturnsAsync("x");
            var redisUptimeMock = new Mock<IRedisUptime>();
            var exception = new Exception("error message");
            redisUptimeMock.Setup(x => x.Read()).ThrowsAsync(exception);
            var check = new RedisIsAvailable(redisVersionMock.Object, redisUptimeMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }
    }
}
