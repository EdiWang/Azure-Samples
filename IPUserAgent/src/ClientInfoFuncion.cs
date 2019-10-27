using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Edi.AzureFunctions
{
    public static class ClientInfoFuncion
    {
        [FunctionName("IP")]
        public static IActionResult GetClientIp(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Requesting client IP.");
            var ip = req.HttpContext.Connection.RemoteIpAddress.ToString();
            return ip != null
                ? (ActionResult)new OkObjectResult($"{ip}")
                : new BadRequestObjectResult("ip is null");
        }

        [FunctionName("UserAgent")]
        public static IActionResult GetClientUserAgent(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Requesting client User-Agent.");
            var ua = req.Headers["User-Agent"].ToString();
            return ua != null
                ? (ActionResult)new OkObjectResult($"{ua}")
                : new BadRequestObjectResult("user-agent is null");
        }
    }
}
