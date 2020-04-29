using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;

namespace MyHttpFunction
{
    public static class EchoIpAddressFunction
    {
        [FunctionName("EchoIpAddressFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var client = new HttpClient();
            var result = await client.GetAsync("http://10.0.0.35/echo");
            var ip = await result.Content.ReadAsStringAsync();
            return new OkObjectResult(ip);
        }
    }
}
