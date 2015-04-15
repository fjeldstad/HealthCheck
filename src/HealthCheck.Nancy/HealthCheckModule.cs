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

        public HealthCheckModule(IEnumerable<IChecker> checkers, HealthCheckOptions options = null)
        {
            if (options == null)
            {
                options = new HealthCheckOptions();
            }
            if (options.RequireHttps)
            {
                this.RequiresHttps(redirect: false);
            }

            Get[options.Route, true] = async (_, ct) =>
            {
                if (options.AuthorizationCallback != null && !await options.AuthorizationCallback(Context))
                {
                    return HttpStatusCode.Unauthorized;
                }
                return Response.AsJson(await (new Core.HealthCheck(checkers).Run()));
            };
        }
    }

    public class HealthCheckOptions
    {
        public string Route { get; set; }
        public bool RequireHttps { get; set; }
        public Func<NancyContext, Task<bool>> AuthorizationCallback { get; set; }

        public HealthCheckOptions()
        {
            Route = "/healthcheck";
            RequireHttps = true;
            AuthorizationCallback = null;
        }
    }
}
