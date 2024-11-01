using Back.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController(IHostEnvironment env) : Controller
    {
        private readonly IHostEnvironment _env = env;

        [HttpGet]
        [Route("/")]
        [SkipApiKey]
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewBag.Environment = _env.EnvironmentName;
            return View();
        }
    }
}
