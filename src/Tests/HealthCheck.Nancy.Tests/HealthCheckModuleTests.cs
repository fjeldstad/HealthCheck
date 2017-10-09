using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCheck.Core;
using Moq;
using Nancy;
using Nancy.Testing;
using Newtonsoft.Json;
using NUnit.Framework;

namespace HealthCheck.Nancy.Tests
{
    [TestFixture]
    public class HealthCheckModuleTests
    {
        [Test]
        public void UsesTheDesiredRoute()
        {
            // Arrange
            var options = new HealthCheckOptions
            {
                Route = "/my/custom/healthcheck"
            };
            var module = new HealthCheckModule(Enumerable.Empty<IChecker>(), options);
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module);
            });
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get(options.Route, with =>
            {
                with.HttpsRequest();
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void CanRequireHttps()
        {
            // Arrange
            var options = new HealthCheckOptions
            {
                RequireHttps = true
            };
            var module = new HealthCheckModule(Enumerable.Empty<IChecker>(), options);
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module);
            });
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/healthcheck", with =>
            {
                with.HttpRequest();
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public void DoesNotHaveToRequireHttps()
        {
            // Arrange
            var options = new HealthCheckOptions
            {
                RequireHttps = false
            };
            var module = new HealthCheckModule(Enumerable.Empty<IChecker>(), options);
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module);
            });
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/healthcheck", with =>
            {
                with.HttpRequest();
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void AllRequestsAuthorizedByDefault()
        {
            // Arrange
            var module = new HealthCheckModule(Enumerable.Empty<IChecker>());
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module);
            });
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/healthcheck", with =>
            {
                with.HttpsRequest();
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [Test]
        public void BlocksUnauthorizedRequests()
        {
            // Arrange
            var options = new HealthCheckOptions
            {
                AuthorizationCallback = context => Task.FromResult(false)
            };
            var module = new HealthCheckModule(Enumerable.Empty<IChecker>(), options);
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module);
            });
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/healthcheck", with =>
            {
                with.HttpsRequest();
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void RunsAllCheckers()
        {
            // Arrange
            var checkerMocks = Enumerable
                .Range(1, 3)
                .Select(x =>
                {
                    var mock = new Mock<IChecker>();
                    mock.Setup(y => y.Check()).ReturnsAsync(new CheckResult());
                    return mock;
                })
                .ToArray();
            var module = new HealthCheckModule(checkerMocks.Select(x => x.Object));
            var bootstrapper = new ConfigurableBootstrapper(with =>
            {
                with.Module(module);
            });
            var browser = new Browser(bootstrapper);

            // Act
            var response = browser.Get("/healthcheck", with =>
            {
                with.HttpsRequest();
            });

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            foreach (var checkerMock in checkerMocks)
            {
                checkerMock.Verify(x => x.Check(), Times.Once);
            }
        }
    }
}
