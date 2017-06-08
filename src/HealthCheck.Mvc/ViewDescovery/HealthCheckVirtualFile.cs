using System.Reflection;

namespace HealthCheck.Mvc.ViewDescovery
{
    public class HealthCheckVirtualFile : System.Web.Hosting.VirtualFile
    {
        public HealthCheckVirtualFile(string virtualPath) : base(virtualPath) { }

        public override System.IO.Stream Open()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("HealthCheck.Mvc.Views.HealthCheck.cshtml");
        }
    }
}