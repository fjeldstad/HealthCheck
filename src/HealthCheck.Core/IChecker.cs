using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public interface IChecker
    {
        string Name { get; set; }
        string SectionName { get; set; }
        bool PreserveContext { get; }
        Task<CheckResult> Check();
    }
}
