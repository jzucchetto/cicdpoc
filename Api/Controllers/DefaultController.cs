using System.Reflection;
using Microsoft.AspNetCore.Mvc;
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
    }
}
