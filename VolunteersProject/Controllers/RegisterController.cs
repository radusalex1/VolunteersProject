using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Repository;
using VolunteersProject.Models;

namespace VolunteersProject.Controllers
{

    public class RegisterController : Controller
    {
        private IVolunteerRepository volunteerRepository;
        private IConfiguration configuration;
        private IUserRepository userRepository;

        public RegisterController(IVolunteerRepository volunteerRepository, IConfiguration configuration, IUserRepository userRepository)
        {
            this.volunteerRepository = volunteerRepository;
            this.configuration = configuration;
            this.userRepository = userRepository;
        }


        // GET: RegisterController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RegisterController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RegisterController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RegisterController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind("Name,Surname,UserName,Password,City,Email,Phone,BirthDate,JoinHubDate,InstagramProfile,FaceBookProfile")] RegisterFormModel registerFormModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var volunteer = new Volunteer
                    {
                        Name = registerFormModel.Name,
                        Surname = registerFormModel.Surname,
                        City = registerFormModel.City,
                        Email = registerFormModel.Email,
                        BirthDate = registerFormModel.BirthDate,
                        JoinHubDate = registerFormModel.JoinHubDate,
                        InstagramProfile = registerFormModel.InstagramProfile,
                        FaceBookProfile = registerFormModel.InstagramProfile
                    };

                    var user = new User
                    {
                        FirstName = registerFormModel.Name,
                        LastName = registerFormModel.Surname,
                        UserName = registerFormModel.UserName,
                        Password = registerFormModel.Password,
                        Role = "User"
                    };

                    userRepository.AddUser(user);
                    volunteerRepository.AddVolunteer(volunteer);
                }
                return View();
            }
            catch
            {
                return View();
            }
        }

        // GET: RegisterController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: RegisterController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: RegisterController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: RegisterController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
