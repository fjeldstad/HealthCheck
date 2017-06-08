using System;

namespace HealthCheck.Core.Results
{
    public interface ICheckResult
    {
        String CheckerName { get; set; }
        String SectionName { get; set; }
        Boolean Passed { get; }
        String Output { get; }
    }
}
