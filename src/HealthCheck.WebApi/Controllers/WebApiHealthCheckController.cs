using System.Web.Http;

namespace HealthCheck.WebApi.Controllers
{
    public class WebApiHealthCheckController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Run()
        {
            var result = new Core.HealthCheck(WebApiHealthCheckСonfigurator.GetInstance().GetCheckers()).Run().Result;
            return Ok(result);
        }
    }
}
