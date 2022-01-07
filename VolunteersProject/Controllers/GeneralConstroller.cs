using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{

    public class GeneralConstroller : Controller
    {
        public readonly ILogger<GeneralConstroller> Logger;
        public IConfiguration configuration;

        public GeneralConstroller(ILogger<GeneralConstroller> logger, IConfiguration configuration)
        {
            this.Logger = logger;
            this.configuration = configuration;
        }

        protected string validateCity(string city)
        {
            if(string.IsNullOrEmpty(city))
            {
                return string.Empty;
            }
            return char.ToUpper(city[0]) + city.Substring(1);
        }

        protected string validateName(string name)
        {
            name = name.ToUpper();
            return name;
        }

        protected bool PhoneNumberIsValid(string phoneNumber)
        {
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

        protected bool InstagramIsValid(string instagramProfile)
        {
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

        protected bool ValidateImageProfile(Volunteer volunteer, int width, int height)
        {
            return (volunteer.ImageProfile.Length > width * height) ? true : false;
        }
    }
}
