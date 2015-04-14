using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public interface IChecker
    {
        string Name { get; }
        Task<CheckResult> Check();
    }
}
