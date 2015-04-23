using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace HealthCheck.Core.Tests
{
    [TestFixture]
    public class CheckerBaseTests
    {
        [Test]
        public async Task CheckFailsOnTimeout()
        {
            // Arrange
            var delay = TimeSpan.FromSeconds(3);
            var checker = new DummyChecker(async () =>
            {
                await Task.Delay(delay);
                return new CheckResult { Passed = true };
            });
            checker.Timeout = TimeSpan.FromSeconds(delay.TotalSeconds / 2);

            // Act
            var result = await checker.Check();

            // Assert
            Assert.That(result.Passed, Is.False);
            Assert.That(result.Output, Is.EqualTo(new TimeoutException().Message));
        }

        public class DummyChecker : CheckerBase
        {
            private readonly Func<Task<CheckResult>> _check;
 
            public override string Name
            {
                get { return "Dummy checker"; }
            }

            public DummyChecker(Func<Task<CheckResult>> check)
            {
                _check = check;
            }

            protected override async Task<CheckResult> CheckCore()
            {
                return await _check().ConfigureAwait(false);
            }
        }
    }
}
