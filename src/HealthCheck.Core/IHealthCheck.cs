using System;
using System.Threading.Tasks;
using HealthCheck.Core.Results;

namespace HealthCheck.Core
{
    public interface IHealthCheck
    {
        Task<HealthCheckResult> Run();
    }
}