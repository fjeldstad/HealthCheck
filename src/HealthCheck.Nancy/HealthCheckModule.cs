using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HealthCheck.Core;
using Nancy;
using Nancy.Security;

namespace HealthCheck.Nancy
{
    public class HealthCheckModule : NancyModule
    {
        public const string DefaultRoute = "/healthcheck";

        public HealthCheckModule(
            IEnumerable<IChecker> checkers, 
            string route = DefaultRoute, 
            bool requireHttps = true,
            Func<NancyContext, Task<bool>> authorized = null,
            Action<Exception> logException = null)
        {
            if (requireHttps)
            {
                this.RequiresHttps();
            }

            Get[route, true] = async (_, ct) =>
            {
                try
                {
                    if (authorized != null && !await authorized(Context))
                    {
                        return HttpStatusCode.Unauthorized;
                    }
                    return Response.AsJson(new HealthCheckResult(await Task.WhenAll(checkers.Select(x => x.Check()))));
                }
                catch (Exception ex)
                {
                    if (logException != null)
                    {
                        try
                        {
                            logException(ex);
                        }
                        catch
                        {
                        }
                    }
                    throw;
                }
            };
        }
    }
}
