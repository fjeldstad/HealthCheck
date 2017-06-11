using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HealthCheck.Core.Configuration;
using HealthCheck.Core.Results;

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
                    return new HealthCheckResult(Enumerable.Empty<ICheckResult>());
                }
                // Run all checks in parallel (if possible). Catch exceptions for each
                // checker individually.
                var checkResults = Checkers.AsParallel()
                    .Select(async x =>
                    {
                        try
                        {
                            return !x.PreserveContext ? await x.Check().ConfigureAwait(false) : x.Check().Result;
                        }
                        catch (Exception ex)
                        {
                            return new CheckResult
                            {
                                CheckerName = x.Name,
                                SectionName = x.SectionName,
                                Passed = false,
                                Output = ex.Message
                            };
                        }
                    }).ToArray();
                await Task.WhenAll(checkResults).ConfigureAwait(false);

                var result = new HealthCheckResult(checkResults.Select(x => x.Result));
                return result;
            }
            catch (AggregateException ex)
            {
                return new HealthCheckResult(ex.Flatten().InnerExceptions.Select(innerEx => new CheckResult
                {
                    CheckerName = innerEx.TargetSite != null && !string.IsNullOrEmpty(innerEx.TargetSite.Name) ?
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
                        CheckerName = "HealthCheck",
                        Passed = false,
                        Output = ex.Message
                    }
                });
            }
        }
    }
}
