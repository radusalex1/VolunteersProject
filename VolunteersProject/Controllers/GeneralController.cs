using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// General Controller
    /// </summary>
    public class GeneralController : Controller
    {
        /// <summary>
        /// Logger.
        /// </summary>
        public readonly ILogger<GeneralController> Logger;

        /// <summary>
        /// configuration
        /// </summary>
        public IConfiguration configuration;

        /// <summary>
        /// User repository.
        /// </summary>
        public readonly IUserRepository userRepository;

        /// <summary>
        /// Current user id.
        /// </summary>
        protected static int currentUserId;

        /// <summary>
        /// Current volunteer id.
        /// </summary>
        protected static int currentVolunteerId;

        //http.context
        //claims principle

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Configuration.</param>
        /// <param name="userRepository">User repository.</param>
        public GeneralController(ILogger<GeneralController> logger, IConfiguration configuration, IUserRepository userRepository)
        {
            this.Logger = logger;
            this.configuration = configuration;

            this.userRepository = userRepository;
        }

        /// <summary>
        /// Not authorized action method.
        /// </summary>
        /// <returns></returns>
        public IActionResult NotAuthorized()
        {
            var errorViewModel = new ErrorViewModel();

            errorViewModel.ErrorMessage = "You are not authorized to view this page. Click back to return to previous page.";

            return View("NotAuthorized", errorViewModel);
        }

        /// <summary>
        /// Method to sort events by criterias.
        /// </summary>
        /// <param name="sortOrder">Sort criteria</param>
        /// <param name="contributions">List of contributions.</param>
        /// <returns>List of sorted contributions.</returns>
        protected static List<Contribution> SortContributions(string sortOrder, List<Contribution> contributions)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    contributions = contributions.OrderByDescending(c => c.Name).ToList();
                    break;
                case "Credits":
                    contributions = contributions.OrderBy(c => c.Credits).ToList();
                    break;
                case "Credits_desc":
                    contributions = contributions.OrderByDescending(c => c.Credits).ToList();
                    break;
                case "sd_asc":
                    contributions = contributions.OrderBy(c => c.StartDate).ToList();
                    break;
                case "sd_desc":
                    contributions = contributions.OrderByDescending(c => c.StartDate).ToList();
                    break;
                case "fd_asc":
                    contributions = contributions.OrderBy(c => c.FinishDate).ToList();
                    break;
                case "fd_desc":
                    contributions = contributions.OrderByDescending(c => c.FinishDate).ToList();
                    break;
                default:
                    contributions = contributions.OrderBy(c => c.Name).ToList();
                    break;
            }

            return contributions;
        }
    }
}
