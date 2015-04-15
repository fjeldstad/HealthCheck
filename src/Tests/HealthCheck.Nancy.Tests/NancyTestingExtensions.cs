using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nancy.Testing;
using Newtonsoft.Json.Linq;

namespace HealthCheck.Nancy.Tests
{
    public static class NancyTestingExtensions
    {
        public static dynamic AsJson(this BrowserResponseBodyWrapper bodyWrapper)
        {
            return JObject.Parse(bodyWrapper.AsString());
        }
    }
}
