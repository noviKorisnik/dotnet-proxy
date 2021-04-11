using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DotnetProxy.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProxyController : ControllerBase
    {
        private readonly ProxyService _service;
        private readonly ILogger<ProxyController> _logger;

        public ProxyController(
            ILogger<ProxyController> logger,
            ProxyService service
            )
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet]
        public object Get(string url)
        {
            return Content(_service.Get(url), "application/json");
        }
    }
}
