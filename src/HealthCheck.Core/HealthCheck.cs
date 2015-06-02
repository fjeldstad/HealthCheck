using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public class HealthCheck
    {
        private readonly IEnumerable<IChecker> _checkers;

        public HealthCheck(IEnumerable<IChecker> checkers)
        {
            _checkers = checkers;
        }

        public async Task<HealthCheckResult> Run()
        {
            try
            {
                if (_checkers == null)
                {
                    return new HealthCheckResult(Enumerable.Empty<CheckResult>());
                }
                // Run all checks in parallel (if possible). Catch exceptions for each
                // checker individually.
                var checkResults = _checkers
                    .AsParallel()
                    .Select(async x =>
                    {
                        try
                        {
                            return await x.Check().ConfigureAwait(false);
                        }
                        catch (Exception ex)
                        {
                            return new CheckResult
                            {
                                Checker = x.Name,
                                Passed = false,
                                Output = ex.Message
                            };
                        }
                    })
                    .ToArray();
                await Task.WhenAll(checkResults).ConfigureAwait(false);
                return new HealthCheckResult(checkResults.Select(x => x.Result));
            }
            catch (AggregateException ex)
            {
                return new HealthCheckResult(ex.Flatten().InnerExceptions.Select(innerEx => new CheckResult
                {
                    Checker = innerEx.TargetSite != null && !string.IsNullOrEmpty(innerEx.TargetSite.Name) ?
                        innerEx.TargetSite.Name :
                        "HealthCheck",
                    Passed = false,
                    Output = innerEx.Message
                }));
            }
            catch (Exception ex)
            {
                // Never let Run throw. Instead, returned a failed HealthCheckResult.
                return new HealthCheckResult(new[]
                {
                    new CheckResult
                    {
                        Checker = "HealthCheck",
                        Passed = false,
                        Output = ex.Message
                    }
                });
            }
        }
    }
}
