using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using HealthCheck.Core;
using HealthCheck.Core.Configuration;

namespace HealthCheck.WebApi
{
    public class WebApiHealthCheckСonfigurator : ConfiguratorBase
    {
        private static WebApiHealthCheckСonfigurator _instance;

        private WebApiHealthCheckСonfigurator() { }

        public static WebApiHealthCheckСonfigurator GetInstance()
        {
            return _instance ?? (_instance = new WebApiHealthCheckСonfigurator());
        }

        public WebApiHealthCheckСonfigurator ConfigureEndpoint(String url)
        {
            GlobalConfiguration.Configure(c =>
            {
                c.Routes.MapHttpRoute(name: "HealthWebApiCheckRoute",
                    routeTemplate: url,
                    defaults: new {controller = "WebApiHealthCheck", action = "Run"});
            });
           
            return this;
        }
    }
}
