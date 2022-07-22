using MailServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using VolunteersProject.Common;
using VolunteersProject.DTO;
using VolunteersProject.Models;
using VolunteersProject.Repository;
using VolunteersProject.Services;
using VolunteersProject.Util;

namespace VolunteersProject.Controllers
{
    public class AccountController : GeneralController
    {
        //private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly IVolunteerRepository _volunteerRepository;
        private IEmailService _emailService;

        private string generatedToken = null;
        private string loggedUser;


        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="volunteerRepository"></param>
        /// <param name="config">Inject config service.</param>
        /// <param name="tokenService">Inject jwt token service.</param>
        /// <param name="userRepository">Inject user repository service.</param>
        /// <param name="logger"></param>
        /// <param name="emailService"></param>
        public AccountController(IVolunteerRepository volunteerRepository,
            IConfiguration config,
            ITokenService tokenService,
            IUserRepository userRepository,
            ILogger<AccountController> logger,
            IEmailService emailService) :
            base(logger, config, userRepository)
        {
            _volunteerRepository = volunteerRepository;
            _tokenService = tokenService;
            //_userRepository = userRepository;
            _emailService = emailService;
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

                    currentUserId = validUser.Id;

                    var CurrentVolunteer = _volunteerRepository.GetVolunteerByUserId(currentUserId);
                    //aici este o problema
                    if (CurrentVolunteer == null)
                    {
                        currentVolunteerId = -1;
                    }
                    else
                    { 
                        currentVolunteerId = CurrentVolunteer.Id;
                    }

                    HttpContext.Session.SetInt32("currentVolunteerId", currentVolunteerId);

                    //HttpContext.Session.

                    //HttpContext.Session.SetString("LoggedUser", $"{validUser.FirstName + " " + validUser.LastName}");

                    return RedirectToAction("MainWindow");
                }
                else
                {
                    return RedirectToAction("Error", new { errorMessage = "Jwt token is null." });
                }
            }
            else
            {
                return RedirectToAction("Error", new { errorMessage = "Wrong user name or password." });
            }
        }

        [Authorize]
        [Route("Mainwindow")]
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

        /// <summary>
        /// Get method for input email for reset password;
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult InputEmailResetPassword()
        {
            return View("RecoverPasswordGetEmail");
        }

        /// <summary>
        /// Post method for input email for reset password
        /// </summary>
        /// <param name="enterEmailForPasswordRecoveryDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult InputEmailResetPassword([Bind("Email")] EnterEmailForPasswordRecoveryDTO enterEmailForPasswordRecoveryDTO)
        {
            var email = enterEmailForPasswordRecoveryDTO.Email;

            if (string.IsNullOrEmpty(email))
            {
                return View("RecoverPasswordGetEmail", enterEmailForPasswordRecoveryDTO);
            }
            if (string.IsNullOrEmpty(email) == false && !Helper.EmailIsValid(email))
            {
                ViewBag.Email_Error = "Wrong Email.";
                return View("RecoverPasswordGetEmail", enterEmailForPasswordRecoveryDTO);
            }
            if (_volunteerRepository.EmailExists(email) == false)
            {
                ViewBag.Email_Error = "Please enter an existing email.";
                return View("RecoverPasswordGetEmail", enterEmailForPasswordRecoveryDTO);
            }

            SendEmail(email);

            return View("Login");
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(int UserId)
        {
            var newPasswordDTO = new NewPasswordDTO()
            {
                UserId = UserId
            };

            return View(newPasswordDTO);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult ResetPassword(int UserId, [Bind("NewPassword,ConfirmNewPassword")] NewPasswordDTO newPasswordDTO)
        {

            if (ModelState.IsValid)
            {
                if (newPasswordDTO.NewPassword != newPasswordDTO.ConfirmNewPassword)
                {
                    ViewBag.NewPassword_Error = "Does not match!";
                    return View(newPasswordDTO);
                }
                userRepository.ChangePasswordBasedOnUserId(UserId, newPasswordDTO.NewPassword);
            }
            else
            {
                return View();
            }

            return View("Login");
        }

        public void SendEmail(string email)
        {
            var emailMessage = new EmailMessage();

            emailMessage.Subject = "Password Reset";

            emailMessage.ToAddresses = new List<EmailAddress>()
            {
                new EmailAddress {Address = email}
            };

            var link = GetLink(email);

            var emailSender = configuration.GetSection("AppSettings").GetSection("EmailSender").Value;

            emailMessage.FromAddresses = new List<EmailAddress>
            {
                    new EmailAddress { Address = emailSender }
            };

            emailMessage.Content = $"Clink on {link} for password reset";

            _emailService.Send(emailMessage);
        }
       
        private string GetLink(string email)
        {

            //localhost: 9307/Enrollments/VolunteerEmailAnswer?contributionId = 4&volunteerId = 35
            //http://localhost:9307/account/resetpassword?email=radus_alexandru@yahoo.com

            var server = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";

            var applicationSiteName = configuration.GetSection("AppSettings").GetSection("ApplicationSiteName").Value;

            var action = $"{applicationSiteName}/Account/ResetPassword?UserId={_volunteerRepository.ReturnUserIdBasedOnEmail(email)}";

            return $"<a href=\"{server}{action}\">link</a>";

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
            //Write your code here to authenticate the user
            return userRepository.GetUser(userModel);
        }
    }
}
