using Back.Middleware;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Back.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        private readonly IHostEnvironment _env;

        public HomeController(IHostEnvironment env)
        {
            _env = env;
        }

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
