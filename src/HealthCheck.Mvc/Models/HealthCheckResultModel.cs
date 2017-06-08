using System;
using System.Collections.Generic;
using System.Linq;
using HealthCheck.Core;
using HealthCheck.Core.Results;

namespace HealthCheck.Mvc.Models
{
    public class HealthCheckResultModel
    {
        public String DateTimeString { get; }
        public Boolean Passed { get; }
        public String Output { get; }
        public HealthCheckResultSummary Summary { get; }
        public IEnumerable<IGrouping<String, ICheckResult>> Sections { get; }
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
                    if (String.IsNullOrEmpty(r.SectionName))
                    {
                        r.SectionName = "#";
                    }
                    return r;
                })
                .Where(r => !String.IsNullOrEmpty(r.SectionName)).ToLookup(r => r.SectionName).OrderBy(gr => gr.Key);
        }
    }

    public class HealthCheckResultSummary
    {
        public Int32 TotalChecks { get; set; }
        public Int32 PassedChecks { get; set; }
        public Int32 FailedChecks { get; set; }
    }
}