using System;

namespace HealthCheck.Core.Results
{
    public interface ICheckResult
    {
        string CheckerName { get; set; }
        string SectionName { get; set; }
        bool Passed { get; }
        string Output { get; }
    }
}
