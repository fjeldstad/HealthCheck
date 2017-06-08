using System;
using System.Reflection;
using System.Web.Caching;
using System.Web.Hosting;

namespace HealthCheck.Mvc.ViewDescovery
{
    public class HealthCheckViewVirtualPathProvider : VirtualPathProvider
    {
        public override bool FileExists(string virtualPath)
        {
            return virtualPath == "/Views/HealthCheck.cshtml" || base.FileExists(virtualPath);
        }

        public override System.Web.Caching.CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPath == "/Views/HealthCheck.cshtml")
            {
                var asm = Assembly.GetExecutingAssembly();
                return new CacheDependency(asm.Location);
            }
            else
            {
                return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
            }
        }

        public override System.Web.Hosting.VirtualFile GetFile(string virtualPath)
        {
            if (virtualPath == "/Views/HealthCheck.cshtml")
            {
                return new HealthCheckVirtualFile(virtualPath);
            }
            else
            {
                return base.GetFile(virtualPath);
            }
        }
    }
}
