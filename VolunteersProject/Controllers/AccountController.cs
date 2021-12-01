using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using VolunteersProject.Common;
using VolunteersProject.DTO;
using VolunteersProject.Models;
using VolunteersProject.Repository;
using VolunteersProject.Services;

namespace VolunteersProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private string generatedToken = null;

        private string loggedUser;

        //private static string _mduDb;


        public AccountController(IConfiguration config, ITokenService tokenService, IUserRepository userRepository)//, MDUOptions options)
        {
            _config = config;
            _tokenService = tokenService;
            _userRepository = userRepository;

            //options.CompanyCode = "bbb";
        }

        [AllowAnonymous]
        //[Route("Login")]
        [HttpGet]
        public IActionResult Login()
        {
            return View("Login");
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpGet]
        public IActionResult ReloadLogin()
        {
            return View("Login");
        }

        [AllowAnonymous]
        [Route("Login")]
        [HttpPost]
        public IActionResult Login(UserModel userModel)
        {
            if (string.IsNullOrEmpty(userModel.UserName) || string.IsNullOrEmpty(userModel.Password))
            {
                return (RedirectToAction("Error"));
            }

            IActionResult response = Unauthorized();
            var validUser = GetUser(userModel);

            if (validUser != null)
            {
                generatedToken = _tokenService.BuildToken(_config["Jwt:Key"].ToString(), _config["Jwt:Issuer"].ToString(), validUser);

                if (generatedToken != null)
                {
                    //HttpContext.Session.SetString("Token", generatedToken);                   
                    ApplicationValues.JwtToken = generatedToken;

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
            //string token = HttpContext.Session.GetString("Token");
            string token = ApplicationValues.JwtToken;

            if (token == null)
            {
                return (RedirectToAction("Index"));
            }

            if (!_tokenService.IsTokenValid(_config["Jwt:Key"].ToString(),
                _config["Jwt:Issuer"].ToString(), token))
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
            //HttpContext.Session.Remove("Token");
            ApplicationValues.JwtToken = string.Empty;

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
                RequestId = "1",                
                ErrorMessage = errorMessage
            };

            return View("Error", errorViewModel);
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

        private UserDTO GetUser(UserModel userModel)
        {
            //todo Radu - move this to repository

            //Write your code here to authenticate the user
            return _userRepository.GetUser(userModel);
        }
    }
}
