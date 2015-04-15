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
    public class SystemDriveHasFreeSpaceTests
    {
        [Test]
        public async Task CheckPassesWhenFreeSpaceIsAboveThreshold()
        {
            // Arrange
            var options = new SystemDriveHasFreeSpace.Options();
            var driveSpaceMock = new Mock<IAvailableSystemDriveSpace>();
            driveSpaceMock.Setup(x => x.Read()).ReturnsAsync(options.SystemDriveAvailableFreeSpaceWarningThresholdInBytes + 1);
            var check = new SystemDriveHasFreeSpace(driveSpaceMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.True);
        }

        [Test]
        public async Task CheckFailsWhenFreeSpaceIsBelowThreshold()
        {
            // Arrange
            var options = new SystemDriveHasFreeSpace.Options();
            var driveSpaceMock = new Mock<IAvailableSystemDriveSpace>();
            driveSpaceMock.Setup(x => x.Read()).ReturnsAsync(options.SystemDriveAvailableFreeSpaceWarningThresholdInBytes - 1);
            var check = new SystemDriveHasFreeSpace(driveSpaceMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
        }

        [Test]
        public async Task CheckFailsWhenFreeSpaceThrows()
        {
            // Arrange
            var exception = new Exception("error message");
            var driveSpaceMock = new Mock<IAvailableSystemDriveSpace>();
            driveSpaceMock.Setup(x => x.Read()).ThrowsAsync(exception);
            var check = new SystemDriveHasFreeSpace(driveSpaceMock.Object);

            // Act
            var result = await check.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(exception.Message));
        }
    }
}
