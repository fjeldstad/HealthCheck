using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Web.Routing;
using HealthCheck.Core;
using HealthCheck.Core.Configuration;
using HealthCheck.Mvc.ViewDiscovery;

namespace HealthCheck.Mvc
{
    public class MvcHealthCheckСonfigurator : ConfiguratorBase
    {
        private static MvcHealthCheckСonfigurator _instance;

        private MvcHealthCheckСonfigurator() { }

        public static MvcHealthCheckСonfigurator GetInstance()
        {
            return _instance ?? (_instance = new MvcHealthCheckСonfigurator());
        }

        public MvcHealthCheckСonfigurator ConfigureEndpoint(RouteCollection routeCollection, string url)
        {
            routeCollection.MapRoute(name: "MvcHealthCheckRoute",
               url: url,
               defaults: new { controller = "MvcHealthCheck", action = "Run" },
               namespaces: new[] { "HealthCheck.Mvc.Controllers" });

            HostingEnvironment.RegisterVirtualPathProvider(new HealthCheckViewVirtualPathProvider());

            return this;
        }
    }
}