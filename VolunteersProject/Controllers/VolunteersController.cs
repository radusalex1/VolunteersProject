using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Volunteer controller.
    /// </summary>
    [Authorize]
    public class VolunteersController : GeneralController
    { 
        public IVolunteerRepository volunteerRepository;
        public readonly IUserRepository userRepository;

        private int pageSize;
        private int imgWidth;
        private int imgHeight;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="volunteerRepository">Volunteer repository.</param>
        /// <param name="userRepository"></param>
        /// <param name="configuration">Application configuration.</param>
        public VolunteersController(ILogger<VolunteersController> logger, IConfiguration configuration, 
            IVolunteerRepository volunteerRepository,IUserRepository userRepository) : base(logger, configuration)
        {
            this.volunteerRepository = volunteerRepository;
            this.userRepository = userRepository;
            pageSize = Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("PageSize").Value);
            imgWidth = Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("ImageProfileWidth").Value);
            imgHeight = Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("ImageProfileHeight").Value);
        }

        /// <summary>
        /// Display a list of volunteers.
        /// </summary>
        /// <param name="sortOrder">Sort order.</param>
        /// <param name="SearchString">Search string.</param>
        /// <param name="currentFilter">Current filter.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <returns></returns>
        /// GET: Volunteers
        [Authorize(Roles = Common.Role.Admin + "," + Common.Role.User)]
        public IActionResult Index(string sortOrder, string SearchString, string currentFilter, int? pageNumber)
        {
            
            this.Logger.LogInformation("HttpGet VolunteersContr Index()");

            ViewData["CurrentSort"] = sortOrder;

            ViewData["FullNameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["AgeSortParam"] = sortOrder == "Age" ? "Age_desc" : "Age";
            ViewData["CitySortParam"] = sortOrder == "City" ? "City_desc" : "City";
            ViewData["JoinHubDateParam"] = sortOrder == "JoinHubDate" ? "JoinHubDate_desc" : "JoinHubDate";

            ViewData["NameFilter"] = SearchString;


            if (SearchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                SearchString = currentFilter;
            }

            ViewData["CurrentFilter"] = SearchString;


            var volunteers = volunteerRepository.GetVolunteers();


            if (!String.IsNullOrEmpty(SearchString))
            {
                volunteers = volunteers.Where(s => s.Name.Contains(SearchString) || s.Surname.Contains(SearchString));
            }

             volunteers = GetSortedVolunteers(sortOrder, volunteers);

            return View(PaginatedList<Volunteer>.Create(volunteers, pageNumber ?? 1, pageSize));
        }

        /// <summary>
        /// Here sort volunteers
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="volunteers"></param>
        /// <returns></returns>
        private IQueryable<Volunteer> GetSortedVolunteers(string sortOrder, IQueryable<Volunteer> volunteers)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    volunteers = volunteers.OrderByDescending(s => s.Name);
                    break;
                case "Age":
                    volunteers = volunteers.OrderBy(s => s.BirthDate);
                    break;
                case "Age_desc":
                    volunteers = volunteers.OrderByDescending(s => s.BirthDate);
                    break;
                case "City_desc":
                    volunteers = volunteers.OrderByDescending(s => s.City);
                    break;
                case "City":
                    volunteers = volunteers.OrderBy(s => s.City);
                    break;
                case "JoinHubDate":
                    volunteers = volunteers.OrderBy(s => s.JoinHubDate);
                    break;
                case "JoinHubDate_desc":
                    volunteers = volunteers.OrderByDescending(s => s.JoinHubDate);
                    break;
                default:
                    volunteers = volunteers.OrderBy(s => s.Name);
                    break;
            }

            return volunteers;
        }

        /// <summary>
        /// Details of volunteer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Volunteers/Details/5        
        public IActionResult Details(int id)
        {
            var volunteer = volunteerRepository.GetVolunteerWithEnrollmentsById(id);

            ViewBag.TotalPoints = volunteerRepository.GetVolunteerTotalPoints(volunteer);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        [Authorize(Roles = Common.Role.Admin)]
        public IActionResult Create()
        {
            var volunteer = new Volunteer
            {
                BirthDate = DateTime.Today,
                JoinHubDate = DateTime.Today
            };
            return View(volunteer);
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Role.Admin)]
        public IActionResult Create([Bind("Id,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub,ImageProfile")] Volunteer volunteer)
        {
            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(volunteer.Phone) && PhoneNumberIsValid(volunteer.Phone) == false)
                {
                    ViewBag.Alert = "Incorrect phone number";
                    return View(volunteer);
                }
                if (!string.IsNullOrEmpty(volunteer.InstagramProfile) && InstagramIsValid(volunteer.InstagramProfile) == false)
                {
                    ViewBag.Alert = "Incorrect Instragram Profile";
                    return View(volunteer);
                }
                if (!string.IsNullOrEmpty(volunteer.Email) && EmailIsValid(volunteer.Email) == false)
                {
                    ViewBag.Alert = "Incorrect Email Adress";
                    return View(volunteer);
                }
                if (volunteerRepository.CheckVolunteerExistByPhoneOrEmail(volunteer))
                {
                    ViewBag.Alert = "Existing Volunteer";
                    return View(volunteer);
                }

                volunteer.City = ValidateCity(volunteer.City);
                volunteer.Name = ValidateName(volunteer.Name);

                if (volunteer.ImageProfile != null)
                {
                    if (ValidateImageProfile(volunteer, imgWidth, imgHeight))
                    {
                        ViewBag.Alert = $"Profile image to big. Please use an image having no more than {imgWidth}*{imgHeight} pixels.";
                        return View(volunteer);
                    }

                    volunteer.ImageProfileByteArray = GetByteArrayFromImage(volunteer.ImageProfile);
                }

                volunteerRepository.AddVolunteer(volunteer);

                ViewBag.Alert = "Volunteer added successfully";

                return RedirectToAction(nameof(Index));
            }

            return View(volunteer);
        }

        [Authorize(Roles = Common.Role.Admin)]
        // GET: Volunteers/Edit/5
        public IActionResult Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = Common.Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, [Bind("Id,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub,ImageProfile")] Volunteer volunteer)
        {
            if (id != volunteer.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    if (!string.IsNullOrEmpty(volunteer.Phone) && PhoneNumberIsValid(volunteer.Phone) == false)
                    {
                        ViewBag.Alert = "Incorrect phone number";
                        return View(volunteer);
                    }
                    if (!string.IsNullOrEmpty(volunteer.InstagramProfile) && InstagramIsValid(volunteer.InstagramProfile) == false)
                    {
                        ViewBag.Alert = "Incorrect Instragram Profile";
                        return View(volunteer);
                    }
                    if (!string.IsNullOrEmpty(volunteer.Email) && EmailIsValid(volunteer.Email) == false)
                    {
                        ViewBag.Alert = "Incorrect Email Adress";
                        return View(volunteer);
                    }
                    if (volunteerRepository.CheckVolunteerExistByPhoneOrEmail(volunteer))
                    {
                        ViewBag.Alert = "Existing Volunteer";
                        return View(volunteer);
                    }

                    volunteer.City = ValidateCity(volunteer.City);
                    volunteer.Name = ValidateName(volunteer.Name);

                    if (volunteer.ImageProfile != null)
                    {
                        if (ValidateImageProfile(volunteer, imgWidth, imgHeight))
                        {
                            ViewBag.Alert = $"Profile image to big. Please use an image having no more than {imgWidth}*{imgHeight} pixels.";
                            return View(volunteer);
                        }

                        volunteer.ImageProfileByteArray = GetByteArrayFromImage(volunteer.ImageProfile);
                    }

                    volunteerRepository.UpdateVolunteer(volunteer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!volunteerRepository.VolunteerExists(volunteer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        /// <summary>
        /// Get volunteer by id.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Volunteer.</returns>
        // GET: Volunteers/Delete/5
        [Authorize(Roles = Common.Role.Admin)]
        public IActionResult Delete(int? id)
        {
            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [Authorize(Roles = Common.Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {

            var volunteer = volunteerRepository.GetVolunteerById(id);

            volunteerRepository.DeleteVolunteer(volunteer);

            var user = userRepository.GetUserById(volunteer.User.Id);

            userRepository.DeteleUser(user);

            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Set the volunteer image profile.
        /// </summary>
        /// <param name="volunteerId">Volunteer id.</param>
        /// <returns>Volunteer image profile as a file.</returns>
        [Authorize(Roles = Common.Role.Admin)]
        [HttpGet]
        public IActionResult SetVolunteerImageProfile(int volunteerId)
        {
            var volunteer = volunteerRepository.GetVolunteerById(volunteerId);

            if (volunteer != null && volunteer.ImageProfileByteArray != null)
            {
                return File(volunteer.ImageProfileByteArray, "image/png");
            }

            return null;
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        /// <summary>
        /// Edit personal info page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult PersonalInfoEdit(int id)
        {
            var currentVolunteer = volunteerRepository.GetVolunteerById(id);

            var currentUser = new CurrentUser()
            {
                Name = currentVolunteer.Name,
                Surname = currentVolunteer.Surname,
                UserName = currentVolunteer.User.UserName,
                City = currentVolunteer.City,
                Email = currentVolunteer.Email,
                Phone = currentVolunteer.Phone,
                BirthDate = currentVolunteer.BirthDate,
                JoinHubDate = currentVolunteer.JoinHubDate,
                InstagramProfile = currentVolunteer.InstagramProfile,
                FaceBookProfile = currentVolunteer.FaceBookProfile,
                DescriptionContributionToHub = currentVolunteer.DescriptionContributionToHub,
                ImageProfile = currentVolunteer.ImageProfile
            };

            return View("Edit_PersonalInfo", currentUser);
          
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PersonalInfoEdit(int id, [Bind("Id,Name,Surname,UserName,City,Phone,Email,BirthDate,JoinHubDate,InstagramProfile,FaceBookProfile,ImageProfile,DescriptionContributionToHub")] CurrentUser currentUser)
        {
            if (id != currentUser.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var ToUpdatevolunteer = volunteerRepository.GetVolunteerById(id);
                var ToUpdateUser = userRepository.GetUserById(ToUpdatevolunteer.User.Id);

                UpdateCurrentVolunteerWithNewDataFields(currentUser, ToUpdatevolunteer);

                ToUpdateUser.UserName = currentUser.UserName;

                if (!string.IsNullOrEmpty(ToUpdatevolunteer.Phone) && PhoneNumberIsValid(ToUpdatevolunteer.Phone) == false)
                {
                    ViewBag.Phone_Error = "Incorrect phone number";
                    return View("Edit_PersonalInfo", currentUser);
                }
                if (!string.IsNullOrEmpty(ToUpdatevolunteer.InstagramProfile) && InstagramIsValid(ToUpdatevolunteer.InstagramProfile) == false)
                {
                    ViewBag.Instagram_Error = "Incorrect Instragram Profile";
                    return View("Edit_PersonalInfo", currentUser);
                }
                if (!string.IsNullOrEmpty(ToUpdatevolunteer.Email) && EmailIsValid(ToUpdatevolunteer.Email) == false)
                {
                    ViewBag.Email_Error = "Incorrect Email Adress";
                    return View("Edit_PersonalInfo", currentUser);
                }

                ///if exist => error
                if (volunteerRepository.CheckVolunteerExistByPhoneOrEmail(ToUpdatevolunteer))
                {
                    ViewBag.ExistingEmailOrPhone = "Existing Email or Phone";
                    return View("Edit_PersonalInfo", currentUser);
                }

                if( !string.IsNullOrEmpty(currentUser.UserName)  && userRepository.AlreadyUserUsername_OnEditPersonalInfo(currentUser))
                {
                    ViewBag.UserName_Error = "Existing Username";
                    return View("Edit_PersonalInfo", currentUser);
                }

                ToUpdatevolunteer.City = ValidateCity(ToUpdatevolunteer.City);
                ToUpdatevolunteer.Name = ValidateName(ToUpdatevolunteer.Name);

                if (ToUpdatevolunteer.ImageProfile != null)
                {
                    if (ValidateImageProfile(ToUpdatevolunteer, imgWidth, imgHeight))
                    {
                        ViewBag.Image_Error = $"Profile image to big. Please use an image having no more than {imgWidth}*{imgHeight} pixels.";
                        return View("Edit_PersonalInfo", currentUser);
                    }

                    ToUpdatevolunteer.ImageProfileByteArray = GetByteArrayFromImage(ToUpdatevolunteer.ImageProfile);
                }

                volunteerRepository.UpdateVolunteer(ToUpdatevolunteer);
                userRepository.UpdateUser(ToUpdateUser);

                return RedirectToAction(nameof(Index));

                //return RedirectToAction("")
               
            }
            return View("Edit_PersonalInfo", currentUser);
        }

        private static void UpdateCurrentVolunteerWithNewDataFields(CurrentUser currentUser, Volunteer ToUpdatevolunteer)
        {
            ToUpdatevolunteer.Name = currentUser.Name;
            ToUpdatevolunteer.Surname = currentUser.Surname;
            ToUpdatevolunteer.City = currentUser.City;
            ToUpdatevolunteer.Phone = currentUser.Phone;
            ToUpdatevolunteer.Email = currentUser.Email;
            ToUpdatevolunteer.BirthDate = currentUser.BirthDate;
            ToUpdatevolunteer.JoinHubDate = currentUser.JoinHubDate;
            ToUpdatevolunteer.InstagramProfile = currentUser.InstagramProfile;
            ToUpdatevolunteer.FaceBookProfile = currentUser.FaceBookProfile;
            ToUpdatevolunteer.DescriptionContributionToHub = currentUser.DescriptionContributionToHub;
            ToUpdatevolunteer.ImageProfile = currentUser.ImageProfile;
        }
    }
}
