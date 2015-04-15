using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace HealthCheck.Core.Tests
{
    [TestFixture]
    public class HealthCheckTests
    {
        [Test]
        public async Task Run_WhenNullCheckers_SuccessResult()
        {
            // Arrange
            var healthCheck = new HealthCheck(null);

            // Act
            var result = await healthCheck.Run();

            // Assert
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Status, Is.EqualTo("success"));
            Assert.That(result.Results, Is.Empty);
        }

        [Test]
        public async Task Run_WhenNoCheckers_SuccessResult()
        {
            // Arrange
            var healthCheck = new HealthCheck(Enumerable.Empty<IChecker>());

            // Act
            var result = await healthCheck.Run();

            // Assert
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Status, Is.EqualTo("success"));
            Assert.That(result.Results, Is.Empty);
        }

        [Test]
        public async Task Run_RunsAllCheckers()
        {
            // Arrange
            var checkerMocks = Enumerable
                .Range(1, 3)
                .Select(x =>
                {
                    var mock = new Mock<IChecker>();
                    mock.SetupGet(y => y.Name).Returns(x.ToString());
                    mock.Setup(y => y.Check()).ReturnsAsync(new CheckResult());
                    return mock;
                })
                .ToArray();
            var healthCheck = new HealthCheck(checkerMocks.Select(x => x.Object));

            // Act
            await healthCheck.Run();

            // Assert
            foreach (var checkerMock in checkerMocks)
            {
                checkerMock.Verify(x => x.Check(), Times.Once);
            }
        }

        [Test]
        public async Task Run_WhenCheckerThrows_FailureResult()
        {
            // Arrange
            var checkerMocks = Enumerable
                .Range(1, 3)
                .Select(x =>
                {
                    var mock = new Mock<IChecker>();
                    mock.SetupGet(y => y.Name).Returns(x.ToString());
                    mock.Setup(y => y.Check()).ThrowsAsync(new Exception("error " + mock.Object.Name));
                    return mock;
                })
                .ToArray();
            var healthCheck = new HealthCheck(checkerMocks.Select(x => x.Object));

            // Act
            var result = await healthCheck.Run();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Status, Is.EqualTo("failure"));
            Assert.That(result.Results.Length, Is.EqualTo(checkerMocks.Length));
            foreach (var checkerMock in checkerMocks)
            {
                var checkResult = result.Results.Single(x => x.Checker == checkerMock.Object.Name);
                Assert.That(checkResult.Checker, Is.EqualTo(checkerMock.Object.Name));
                Assert.That(checkResult.Passed, Is.False);
                Assert.That(checkResult.Output, Is.EqualTo("error " + checkerMock.Object.Name));
            }
        }

        [Test]
        public async Task Run_WhenAtLeastOneCheckerFails_FailureResult()
        {
            // Arrange
            var checkerMocks = Enumerable
                .Range(1, 3)
                .Select(x =>
                {
                    var mock = new Mock<IChecker>();
                    mock.SetupGet(y => y.Name).Returns(x.ToString());
                    mock.Setup(y => y.Check()).ReturnsAsync(new CheckResult { Passed = true });
                    return mock;
                })
                .ToArray();
            var exception = new Exception("error message");
            checkerMocks[1].Setup(x => x.Check()).ThrowsAsync(exception);
            var healthCheck = new HealthCheck(checkerMocks.Select(x => x.Object));

            // Act
            var result = await healthCheck.Run();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Status, Is.EqualTo("failure"));
            Assert.That(result.Results.Length, Is.EqualTo(checkerMocks.Length));
            Assert.That(result.Results.Count(x => x.Passed), Is.EqualTo(checkerMocks.Length - 1));
        }

        [Test]
        public async Task Run_WhenAllCheckersPass_SuccessResult()
        {
            // Arrange
            var checkerMocks = Enumerable
                .Range(1, 3)
                .Select(x =>
                {
                    var mock = new Mock<IChecker>();
                    mock.SetupGet(y => y.Name).Returns(x.ToString());
                    mock.Setup(y => y.Check()).ReturnsAsync(new CheckResult { Passed = true });
                    return mock;
                })
                .ToArray();
            var healthCheck = new HealthCheck(checkerMocks.Select(x => x.Object));

            // Act
            var result = await healthCheck.Run();

            // Assert
            Assert.That(result.Passed, Is.True);
            Assert.That(result.Status, Is.EqualTo("success"));
            Assert.That(result.Results.Length, Is.EqualTo(checkerMocks.Length));
            Assert.That(result.Results.Count(x => x.Passed), Is.EqualTo(checkerMocks.Length));
        }

        [Test]
        public async Task Run_WhenSomethingOutsideOfTheCheckersThrows_FailureResult()
        {
            // Arrange
            var checkersMock = new Mock<IEnumerable<IChecker>>();
            var exception = new Exception("surprise");
            checkersMock.Setup(x => x.GetEnumerator()).Throws(exception);
            var healthCheck = new HealthCheck(checkersMock.Object);

            // Act
            var result = await healthCheck.Run();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Status, Is.EqualTo("failure"));
            Assert.That(result.Results, Is.Not.Null);
            Assert.That(result.Results.Single().Checker, Is.EqualTo("HealthCheck"));
            Assert.That(result.Results.Single().Passed, Is.False);
            Assert.That(result.Results.Single().Output, Is.EqualTo(exception.Message));
        }
    }
}
