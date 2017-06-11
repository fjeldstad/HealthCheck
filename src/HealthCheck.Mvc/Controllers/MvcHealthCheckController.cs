using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Mvc;
using HealthCheck.Core;
using HealthCheck.Core.Results;
using HealthCheck.Mvc.Models;

namespace HealthCheck.Mvc.Controllers
{
    public class MvcHealthCheckController : AsyncController
    {
        private const string Key = "result";
        private readonly IHealthCheck _healthCheck;

        public MvcHealthCheckController()
        {
            _healthCheck = new Core.HealthCheck(MvcHealthCheckСonfigurator.GetInstance().GetCheckers());
        }

        public void RunAsync()
        {
            AsyncManager.OutstandingOperations.Increment();
            AsyncManager.Parameters[Key] = _healthCheck.Run().Result;
            AsyncManager.OutstandingOperations.Decrement();
        }

        public ActionResult RunCompleted(HealthCheckResult result)
        {            
            var model = new HealthCheckResultModel(result);
            return View("~/Views/HealthCheck.cshtml", model);
        }
    }
}
