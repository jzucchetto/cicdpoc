using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    [Route("/")]
    public class DefaultController : Controller
    {
        private readonly SystemInfo _systemInfo;

        public DefaultController(IOptions<SystemInfo> systemInfo)
        {
            _systemInfo = systemInfo.Value;
        }

        [HttpGet]
        public string Get()
        {
            return $"Application {Assembly.GetEntryAssembly().GetName().Name} running on {_systemInfo.Hostname}";
        }

        [HttpGet]
        [Route("health")]
        public IActionResult HealthCheck()
        {

            if (Engine.IsReady)
            {
                Console.Out.WriteLine("Health check returned OK");
                return Ok("OK");
            }

            Console.Out.WriteLine("Health check returned Not Found");
            return NotFound();
        }

        [HttpGet]
        [Route("ready")]
        public IActionResult ReadyCheck()
        {
            if (Engine.IsReady)
            {
                Console.Out.WriteLine("Ready check returned OK");
                return Ok();
            }

            Console.Out.WriteLine("Ready check returned Not Found");
            return NotFound();
        }
    }
}
