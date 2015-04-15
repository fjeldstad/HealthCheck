using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCheck.Windows.Checkers;
using HealthCheck.Windows.Metrics;
using Moq;
using NUnit.Framework;

namespace HealthCheck.Windows.Tests.Checkers
{
    [TestFixture]
    public class CpuUsageIsOkTests
    {
        [Test]
        public async Task CheckPassesWhenCpuUsageIsBelowThreshold()
        {
            // Arrange
            var options = new CpuUsageIsOk.Options();
            var cpuUsageMock = new Mock<ICpuUsage>();
            cpuUsageMock.Setup(x => x.Read()).ReturnsAsync(options.CpuUsageWarningThresholdInPercent - 1);
            var check = new CpuUsageIsOk(cpuUsageMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.True);
        }

        [Test]
        public async Task CheckFailsWhenCpuUsageIsAboveThreshold()
        {
            // Arrange
            var options = new CpuUsageIsOk.Options();
            var cpuUsageMock = new Mock<ICpuUsage>();
            cpuUsageMock.Setup(x => x.Read()).ReturnsAsync(options.CpuUsageWarningThresholdInPercent + 1);
            var check = new CpuUsageIsOk(cpuUsageMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
        }

        [Test]
        public async Task CheckFailsWhenCpuUsageThrows()
        {
            // Arrange
            var exception = new Exception("error message");
            var cpuUsageMock = new Mock<ICpuUsage>();
            cpuUsageMock.Setup(x => x.Read()).ThrowsAsync(exception);
            var check = new CpuUsageIsOk(cpuUsageMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }
    }
}
