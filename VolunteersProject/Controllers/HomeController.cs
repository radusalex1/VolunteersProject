using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Contructor
    /// </summary>
    public class HomeController : GeneralController
    {
        private IVolunteerRepository volunteerRepository;

        public HomeController(
            IVolunteerRepository volunteerRepository,
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IUserRepository userRepository)
            : base(logger, configuration, userRepository)
        {
            this.volunteerRepository = volunteerRepository;
        }

        [Authorize]
        public IActionResult Index(string sortOrder)
        {
            Logger.LogInformation("HttpGet Index()");

            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CreditsSortParam"] = sortOrder == "Credits" ? "Credits_desc" : "Credits";
            ViewData["StartDateSortParam"] = sortOrder == "sd_asc" ? "sd_desc" : "sd_asc";
            ViewData["FinishDateSortParam"] = sortOrder == "fd_asc" ? "fd_desc" : "fd_asc";

            int LoggedUserId = currentUserId;

            var CurrentVolunteer = volunteerRepository.GetVolunteerByUserId(LoggedUserId);

            var Contributions = volunteerRepository.GetContributionsByVolunteer(CurrentVolunteer);

            Contributions = SortContributions(sortOrder, Contributions);

            ViewBag.TotalPoints = volunteerRepository.GetVolunteerTotalPoints(CurrentVolunteer);

            return View("HomeView", Contributions);
        }

    }
}
