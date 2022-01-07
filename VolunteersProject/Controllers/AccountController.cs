using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Linq;
using VolunteersProject.Common;
using VolunteersProject.DTO;
using VolunteersProject.Models;
using VolunteersProject.Repository;
using VolunteersProject.Services;
using Controller = Microsoft.AspNetCore.Mvc.Controller;

namespace VolunteersProject.Controllers
{
    public class AccountController : GeneralConstroller
    {
        
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IVolunteerRepository _volunteerRepository;
        private string generatedToken = null;
        private string loggedUser;
        

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="config">Inject config service.</param>
        /// <param name="tokenService">Inject jwt token service.</param>
        /// <param name="userRepository">Inject user repository service.</param>
        public AccountController(IVolunteerRepository volunteerRepository,IConfiguration config, ITokenService tokenService, IUserRepository userRepository, ILogger<AccountController> logger):
            base(logger,config)
        {
            _volunteerRepository = volunteerRepository;
            _tokenService = tokenService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <returns>Load Login view.</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            
            this.Logger.LogInformation("HttpGet Login()");

            return View("Login");
        }

        /// <summary>
        /// Reload login.
        /// </summary>
        /// <returns>Load Login view.</returns>
        [AllowAnonymous]
        [Route("Login")]
        [HttpGet]
        public IActionResult ReloadLogin()
        {
            this.Logger.LogInformation("HttpGet ReloadLogin()");

            return View("Login");
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(LoginModel userModel)
        {
            this.Logger.LogInformation("httpPost Login()");

            if (string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
            {
                return (RedirectToAction("Error"));
            }

            IActionResult response = Unauthorized();
            var validUser = GetUser(userModel);

            if (validUser != null)
            {
                generatedToken = _tokenService.BuildToken(configuration["Jwt:Key"].ToString(), configuration["Jwt:Issuer"].ToString(), validUser);

                if (generatedToken != null)
                {
                    ApplicationValues.JwtToken = generatedToken;
                    HttpContext.Session.SetString("LoggedUser", validUser.UserName);

                    currentUser.User = validUser;
                    currentUser.Volunteer = _volunteerRepository.GetVolunteerByUserId(currentUser.User.Id);

                    return RedirectToAction("MainWindow");
                }
                else
                {                    
                    return RedirectToAction("Error", new { errorMessage = "Jwt tokem is null." });
                }
            }
            else
            {                
                return RedirectToAction("Error", new { errorMessage = "Wrong user name or password." });
            }
        }

        [Authorize]
        [Route("mainwindow")]
        [HttpGet]
        public IActionResult MainWindow()
        {
            string token = ApplicationValues.JwtToken;

            if (token == null)
            {
                return (RedirectToAction("Index"));
            }

            if (!_tokenService.IsTokenValid(configuration["Jwt:Key"].ToString(),
                configuration["Jwt:Issuer"].ToString(), token))
            {
                return (RedirectToAction("Index"));
            }

            ViewBag.Message = BuildMessage(token, 50);

            return RedirectToAction("HomeIndex", "Home");
        }

        /// <summary>
        /// Logout.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            this.Logger.LogInformation("HttpGet Logout()");

            ApplicationValues.JwtToken = string.Empty;
            HttpContext.Session.SetString("userIsLogged", "false");
            HttpContext.Session.SetString("LoggedUser", string.Empty);

            return View("Login");
        }

        /// <summary>
        /// Display the error view
        /// </summary>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public IActionResult Error(string errorMessage)
        {
            var errorViewModel = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                ErrorMessage = errorMessage
            };

            return View("Error", errorViewModel);
        }

        
        public IActionResult SetLoggingAction()
        {
            var action = string.Empty;
            
            if (!string.IsNullOrEmpty(generatedToken))
            {
                action = "ReloadLogin";
            }
            else
            {
                action = "Logout";
            }

            return Content($"<a class=\"nav-link text-dark\" asp-area=\"\" asp-controller=\"Account\" asp-action=\"{ action}\">Login</a>");
        }


        private string BuildMessage(string stringToSplit, int chunkSize)
        {
            var data = Enumerable.Range(0, stringToSplit.Length / chunkSize)
                .Select(i => stringToSplit.Substring(i * chunkSize, chunkSize));

            string result = "The generated token is:";

            foreach (string str in data)
            {
                result += Environment.NewLine + str;
            }

            return result;
        }

        private User GetUser(LoginModel userModel)
        {
            //todo Radu - move this to repository

            //Write your code here to authenticate the user
            return _userRepository.GetUser(userModel);
        }
    }
}
