using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Contructor
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }       

        [Authorize]
        public IActionResult HomeIndex()
        {
            _logger.LogInformation("HttpGet HomeIndex()");

            return View("HomeView");
        }
    }
}
