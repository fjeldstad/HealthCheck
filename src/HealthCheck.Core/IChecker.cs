using System.Collections.Generic;
using System.Threading.Tasks;
using HealthCheck.Core.Results;

namespace HealthCheck.Core
{
    public interface IChecker
    {
        string Name { get; set; }
        string SectionName { get; set; }
        bool PreserveContext { get; }
        Task<ICheckResult> Check();
    }
}
