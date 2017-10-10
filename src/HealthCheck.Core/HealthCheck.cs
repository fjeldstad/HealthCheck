using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core.Configuration;

namespace HealthCheck.Core
{
    public class HealthCheck : HealthCheckBase
    {
        public HealthCheck(IEnumerable<IChecker> checkers) : base(checkers) { }

        public override async Task<HealthCheckResult> Run()
        {
            try
            {
                if (Checkers == null)
                {
                    return new HealthCheckResult(Enumerable.Empty<CheckResult>());
                }
                // Run all checks in parallel (if possible). Catch exceptions for each
                // checker individually.
                var checkResults = Checkers.AsParallel()
                    .Select(async x =>
                    {
                        try
                        {
                            return await x.Check().ConfigureAwait(x.PreserveContext);
                        }
                        catch (Exception ex)
                        {
                            return new CheckResult
                            {
                                Checker = x.Name,
                                SectionName = x.SectionName,
                                Passed = false,
                                Output = ex.Message
                            };
                        }
                    }).ToArray();
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
