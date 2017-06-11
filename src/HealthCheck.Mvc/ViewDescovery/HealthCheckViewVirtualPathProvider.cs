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
            return virtualPath.EndsWith("/Views/HealthCheck.cshtml") || base.FileExists(virtualPath);
        }

        public override CacheDependency GetCacheDependency(string virtualPath, System.Collections.IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPath.EndsWith("/Views/HealthCheck.cshtml"))
            {
                var asm = Assembly.GetExecutingAssembly();
                return new CacheDependency(asm.Location);
            }
            return base.GetCacheDependency(virtualPath, virtualPathDependencies, utcStart);
        }

        public override VirtualFile GetFile(string virtualPath)
        {
            if (virtualPath.EndsWith("/Views/HealthCheck.cshtml"))
            {
                return new HealthCheckVirtualFile(virtualPath);
            }
            return base.GetFile(virtualPath);
        }
    }
}
