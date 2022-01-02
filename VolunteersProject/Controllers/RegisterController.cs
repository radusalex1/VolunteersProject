using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using VolunteersProject.Repository;
using VolunteersProject.Models;
using Microsoft.Extensions.Logging;

namespace VolunteersProject.Controllers
{

    public class RegisterController : GeneralConstroller
    {
        private IVolunteerRepository volunteerRepository;
        private IConfiguration configuration;
        private IUserRepository userRepository;

        public RegisterController(IVolunteerRepository volunteerRepository, IUserRepository userRepository,ILogger<RegisterController> logger, IConfiguration configuration):base(logger,configuration)
        {
            this.volunteerRepository = volunteerRepository;
            this.userRepository = userRepository;
        }

        // GET: RegisterController
        public ActionResult Index()
        {
            return View();
        }
        public IActionResult Create()
        {
            return View();
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

                    if(!string.IsNullOrEmpty(newUser.Phone) && PhoneNumberIsValid(newUser.Phone))
                    {
                        ViewBag.Phone_Error = "Incorrect phone number";
                        return View(newUser);
                    }
                    
                    if(!string.IsNullOrEmpty(newUser.InstagramProfile)&& InstagramIsValid(newUser.InstagramProfile))
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
                        Role = "User"
                    }; 
                    
                    if(userRepository.AlreadyUseUsername(user.UserName))
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
                        UserID = userId
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
