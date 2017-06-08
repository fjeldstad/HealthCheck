using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace HealthCheck.WebApi.Controllers
{
    public class WebApiHealthCheckController : ApiController
    {
        [HttpGet]
        public IHttpActionResult Run()
        {
            var result = new Core.HealthCheck(WebApiHealthCheckСonfigurator.GetInstance().GetCheckers()).Run().Result;
            return ResponseMessage(Request.CreateResponse(result.Passed 
                ? HttpStatusCode.OK 
                : HttpStatusCode.InternalServerError, result));
        }
    }
}
