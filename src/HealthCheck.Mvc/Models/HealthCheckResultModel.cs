using System;
using System.Collections.Generic;
using System.Linq;
using HealthCheck.Core;
using HealthCheck.Core.Results;

namespace HealthCheck.Mvc.Models
{
    public class HealthCheckResultModel
    {
        public string DateTimeString { get; }
        public bool Passed { get; }
        public string Output { get; }
        public HealthCheckResultSummary Summary { get; }
        public IEnumerable<IGrouping<string, ICheckResult>> Sections { get; }
        public HealthCheckResultModel(HealthCheckResult result)
        {
            DateTimeString = DateTime.UtcNow.ToString("G");
            Output = result.Output;
            Passed = result.Passed;
            Summary = new HealthCheckResultSummary
            {
                TotalChecks = result.Results.Length,
                PassedChecks = result.Results.Count(r => r.Passed),
                FailedChecks = result.Results.Count(r => !r.Passed),
            };
            Sections = result.Results
                .Select(r =>
                {
                    if (string.IsNullOrEmpty(r.SectionName))
                    {
                        r.SectionName = "#";
                    }
                    return r;
                })
                .Where(r => !string.IsNullOrEmpty(r.SectionName)).ToLookup(r => r.SectionName).OrderBy(gr => gr.Key);
        }
    }

    public class HealthCheckResultSummary
    {
        public int TotalChecks { get; set; }
        public int PassedChecks { get; set; }
        public int FailedChecks { get; set; }
    }
}