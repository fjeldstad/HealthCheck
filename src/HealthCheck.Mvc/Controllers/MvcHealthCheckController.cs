using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using HealthCheck.Core;
using HealthCheck.Mvc.Models;

namespace HealthCheck.Mvc.Controllers
{
    public class MvcHealthCheckController : Controller
    {
        public ActionResult Run()
        {
            var result = new Core.HealthCheck(MvcHealthCheckСonfigurator.GetInstance().GetCheckers()).Run().Result;
            var model = new HealthCheckResultModel(result);
            return View("/Views/HealthCheck.cshtml", model);
        }
    }
}
