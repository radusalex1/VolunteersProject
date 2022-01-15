using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Contructor
    /// </summary>
    public class HomeController : GeneralConstroller
    {

        private IVolunteerRepository volunteerRepository;

        public HomeController(IVolunteerRepository volunteerRepository,ILogger<HomeController> logger, IConfiguration configuration) :base(logger,configuration)
        {
            this.volunteerRepository = volunteerRepository;
        }       

        [Authorize]
        public IActionResult HomeIndex()
        {
            Logger.LogInformation("HttpGet HomeIndex()");

            int LoggedUserId = currentUserId;

            var CurrentVolunteer = volunteerRepository.GetVolunteerByUserId(LoggedUserId);

            var Contributions = volunteerRepository.GetContributionsByVolunteer(CurrentVolunteer) ;

            ViewBag.TotalPoints = volunteerRepository.GetVolunteerTotalPoints(CurrentVolunteer);

            return View("HomeView",Contributions);
        }
    }
}
