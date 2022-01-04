﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VolunteersProject.Repository;
using VolunteersProject.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace VolunteersProject.Controllers
{
    [AllowAnonymous]
    public class RegisterController : GeneralConstroller
    {
        private IVolunteerRepository volunteerRepository;
        private IUserRepository userRepository;
        private IRolesRepository rolesRepository;

        public RegisterController(IVolunteerRepository volunteerRepository, IUserRepository userRepository, IRolesRepository rolesRepository, ILogger<RegisterController> logger, IConfiguration configuration)
            : base(logger, configuration)
        {
            this.volunteerRepository = volunteerRepository;
            this.userRepository = userRepository;
            this.rolesRepository = rolesRepository;
        }
        
        // GET: RegisterController
        public ActionResult Index()
        {
            return View();
        }

        public IActionResult Create()
        {
            var newUser = new RegisterFormModel
            {
                BirthDate = System.DateTime.Today,
                JoinHubDate = System.DateTime.Today
            };

            return View(newUser);
        }


        // POST: RegisterController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Name,Surname,UserName,Password,City,Email,Phone,BirthDate,JoinHubDate,InstagramProfile,FaceBookProfile")] RegisterFormModel newUser)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (!string.IsNullOrEmpty(newUser.Phone) && PhoneNumberIsValid(newUser.Phone))
                    {
                        ViewBag.Phone_Error = "Incorrect phone number";
                        return View(newUser);
                    }

                    if (!string.IsNullOrEmpty(newUser.InstagramProfile) && InstagramIsValid(newUser.InstagramProfile))
                    {
                        ViewBag.Insta_Error = "Incorrect Instragram Profile";
                        return View(newUser);
                    }

                    if (!string.IsNullOrEmpty(newUser.Email) && EmailIsValid(newUser.Email) == false)
                    {
                        ViewBag.Email_Error = "Incorrect Email Adress";
                        return View(newUser);
                    }

                    var user = new User
                    {
                        FirstName = newUser.Name,
                        LastName = newUser.Surname,
                        UserName = newUser.UserName,
                        Password = newUser.Password,
                        Role = rolesRepository.GetUserRight()
                    };

                    if (userRepository.AlreadyUseUsername(user.UserName))
                    {
                        ViewBag.Username_Error = "Username Already Use";
                        return View(newUser);
                    }

                    int userId = userRepository.AddUser(user);

                    var volunteer = new Volunteer
                    {
                        Name = validateName(newUser.Name),
                        Surname = newUser.Surname,
                        City = validateCity(newUser.City),
                        Email = newUser.Email,
                        BirthDate = newUser.BirthDate,
                        JoinHubDate = newUser.JoinHubDate,
                        InstagramProfile = newUser.InstagramProfile,
                        FaceBookProfile = newUser.InstagramProfile,
                        User = user
                    };

                    volunteerRepository.AddVolunteer(volunteer);
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

    }
}
