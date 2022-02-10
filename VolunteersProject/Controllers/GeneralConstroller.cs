using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// General Controller
    /// </summary>
    public class GeneralConstroller : Controller
    {
        public readonly ILogger<GeneralConstroller> Logger;
        public IConfiguration configuration;
        protected static int currentUserId;
        protected static CurrentUser currentUser;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public GeneralConstroller(ILogger<GeneralConstroller> logger, IConfiguration configuration)
        {
            this.Logger = logger;
            this.configuration = configuration;
        }

        /// <summary>
        /// Validates city(to capitals)
        /// </summary>
        /// <param name="city"></param>
        /// <returns></returns>
        protected string ValidateCity(string city)
        {
            if(string.IsNullOrEmpty(city))
            {
                return string.Empty;
            }
            return char.ToUpper(city[0]) + city.Substring(1);
        }

        /// <summary>
        /// Validates Name(all capitals)
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected string ValidateName(string name)
        {
            name = name.ToUpper();
            return name;
        }

        /// <summary>
        /// Validates Phone number
        /// </summary>
        /// <param name="phoneNumber"></param>
        /// <returns></returns>
        protected bool PhoneNumberIsValid(string phoneNumber)
        {
            //to do: move this to appConfig
            string pattern = @"^\s*(?:\+?(\d{1,3}))?[-. (]*(\d{3})[-. )]*(\d{3})[-. ]*(\d{4})(?: *x(\d+))?\s*$";
            Match m = Regex.Match(phoneNumber, pattern);
            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Validates Instagram
        /// </summary>
        /// <param name="instagramProfile"></param>
        /// <returns></returns>
        protected bool InstagramIsValid(string instagramProfile)
        {
            //to do: move this to appConfig
            string pattern = @"(?:^|[^\w])(?:@)([A-Za-z0-9_](?:(?:[A-Za-z0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z0-9_]))?)";
            Match m = Regex.Match(instagramProfile, pattern);

            if (m.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true is email is valid, false otherwise
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        protected bool EmailIsValid(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false;
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Validates Image profile
        /// </summary>
        /// <param name="volunteer"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        protected bool ValidateImageProfile(Volunteer volunteer, int width, int height)
        {
            return (volunteer.ImageProfile.Length > width * height) ? true : false;
        }

        /// <summary>
        /// Method to sort events by criterias.
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="contributions"></param>
        /// <returns></returns>
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
