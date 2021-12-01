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
        //todo Radu - use logger everywhere
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }       

        [Authorize]
        public IActionResult HomeIndex()
        {            
            return View("HomeView");
        }

        //todo cia - check if this is needed
        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
