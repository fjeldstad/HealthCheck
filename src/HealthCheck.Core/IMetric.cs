using System.Threading.Tasks;

namespace HealthCheck.Core
{
    public interface IMetric<T>
    {
        Task<T> Read();
    }
}
