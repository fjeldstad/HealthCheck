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
            Func<NancyContext, Task<bool>> authorized = null)
        {
            if (requireHttps)
            {
                this.RequiresHttps();
            }

            Get[route, true] = async (_, ct) =>
            {
                if (authorized != null && !await authorized(Context))
                {
                    return HttpStatusCode.Unauthorized;
                }
                return Response.AsJson(await (new Core.HealthCheck(checkers).Run()));
            };
        }
    }
}
