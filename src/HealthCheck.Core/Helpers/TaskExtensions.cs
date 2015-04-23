using System;
using System.Threading.Tasks;

namespace HealthCheck.Core.Helpers
{
    public static class TaskExtensions
    {
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task, TimeSpan timeout)
        {
            if (task == await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false))
            {
                return await task.ConfigureAwait(false);
            }
            throw new TimeoutException();
        }
    }
}
