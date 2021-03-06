using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Models;
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
        public IActionResult HomeIndex(string sortOrder )
        {
            Logger.LogInformation("HttpGet HomeIndex()");

            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CreditsSortParam"] = sortOrder == "Credits" ? "Credits_desc" : "Credits";
            ViewData["StartDateSortParam"] = sortOrder == "sd_asc" ? "sd_desc" : "sd_asc";
            ViewData["FinishDateSortParam"] = sortOrder == "fd_asc" ? "fd_desc" : "fd_asc";

            int LoggedUserId = currentUserId;

            var CurrentVolunteer = volunteerRepository.GetVolunteerByUserId(LoggedUserId);

            var Contributions = volunteerRepository.GetContributionsByVolunteer(CurrentVolunteer) ;

            Contributions = SortContributions(sortOrder, Contributions);

            ViewBag.TotalPoints = volunteerRepository.GetVolunteerTotalPoints(CurrentVolunteer);

            return View("HomeView",Contributions);
        }

    }
}
