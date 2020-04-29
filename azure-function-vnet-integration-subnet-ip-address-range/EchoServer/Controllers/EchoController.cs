using Microsoft.AspNetCore.Mvc;

namespace EchoServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EchoController : ControllerBase
    {
        [HttpGet]
        public string Echo()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
    }
}