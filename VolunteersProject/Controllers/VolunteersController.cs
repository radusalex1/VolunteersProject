using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using VolunteersProject.Common;
using VolunteersProject.Filters;
using VolunteersProject.Models;
using VolunteersProject.Repository;
using VolunteersProject.Util;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Volunteer controller.
    /// </summary>
    //[Authorize]
    //[VolunteersCustomAuthorization]
    public class VolunteersController : GeneralController
    { 
        /// <summary>
        /// Interface for volunteer repository
        /// </summary>
        public IVolunteerRepository volunteerRepository;        

        private int pageSize;
        private int imageFileLengthLimit;        

        /// <summary>
        /// Contructor.
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="volunteerRepository">Volunteer repository.</param>
        /// <param name="userRepository"></param>
        /// <param name="configuration">Application configuration.</param>
        public VolunteersController(
            ILogger<VolunteersController> logger, 
            IConfiguration configuration,            
            IUserRepository userRepository,
            IVolunteerRepository volunteerRepository) 
            : base(logger, configuration, userRepository)
        {
            this.volunteerRepository = volunteerRepository;
            
            pageSize = Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("PageSize").Value);

            imageFileLengthLimit = Convert.ToInt32(configuration.GetSection("AppSettings").GetSection("imageFileLengthLimit").Value);           
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
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.User)]
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

        /// <summary>
        /// Create votunteer.
        /// </summary>
        /// <returns></returns>
        // GET: Volunteers/Create        
        //[VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        //public IActionResult Create()
        //{
        //    var volunteer = new Volunteer
        //    {
        //        BirthDate = DateTime.Today,
        //        JoinHubDate = DateTime.Today
        //    };
        //    return View(volunteer);
        //}

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.       
        //[HttpPost]
        //[ValidateAntiForgeryToken]       
        //[VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        //public IActionResult Create([Bind("Id,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub,ImageProfile")] Volunteer volunteer)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        if (!string.IsNullOrEmpty(volunteer.Phone) && Helper.PhoneNumberIsValid(volunteer.Phone) == false)
        //        {
        //            ViewBag.Alert = "Incorrect phone number";
        //            return View(volunteer);
        //        }
        //        if (!string.IsNullOrEmpty(volunteer.InstagramProfile) && Helper.InstagramIsValid(volunteer.InstagramProfile) == false)
        //        {
        //            ViewBag.Alert = "Incorrect Instragram Profile";
        //            return View(volunteer);
        //        }
        //        if (!string.IsNullOrEmpty(volunteer.Email) && Helper.EmailIsValid(volunteer.Email) == false)
        //        {
        //            ViewBag.Alert = "Incorrect Email Adress";
        //            return View(volunteer);
        //        }
        //        if (volunteerRepository.CheckVolunteerExistByPhoneOrEmail(volunteer))
        //        {
        //            ViewBag.Alert = "Existing Volunteer";
        //            return View(volunteer);
        //        }

        //        volunteer.City = Helper.ValidateCity(volunteer.City);
        //        volunteer.Name = Helper.ValidateName(volunteer.Name);

        //        if (volunteer.ImageProfile != null)
        //        {
        //            if (Helper.ValidateImageProfile(volunteer.ImageProfile.Length, imageFileLengthLimit))
        //            {
        //                ViewBag.Alert = $"Profile image to big. Please use an image file having no more than {imageFileLengthLimit} bytes.";
        //                return View(volunteer);
        //            }

        //            volunteer.ImageProfileByteArray = GetByteArrayFromImage(volunteer.ImageProfile);
        //        }

        //        volunteerRepository.AddVolunteer(volunteer);

        //        ViewBag.Alert = "Volunteer added successfully";

        //        return RedirectToAction(nameof(Index));
        //    }

        //    return View(volunteer);
        //}

        /// <summary>
        /// Edit volunteer.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns></returns>
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        // GET: Volunteers/Edit/5
        public IActionResult Edit(int id)
        {
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
        //[System.Web.Http.Authorize(Roles = Common.Role.Admin)]
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
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
                    if (!string.IsNullOrEmpty(volunteer.Phone) && Helper.PhoneNumberIsValid(volunteer.Phone) == false)
                    {
                        ViewBag.Alert = "Incorrect phone number";
                        return View(volunteer);
                    }
                    if (!string.IsNullOrEmpty(volunteer.InstagramProfile) && Helper.InstagramIsValid(volunteer.InstagramProfile) == false)
                    {
                        ViewBag.Alert = "Incorrect Instragram Profile";
                        return View(volunteer);
                    }
                    if (!string.IsNullOrEmpty(volunteer.Email) && Helper.EmailIsValid(volunteer.Email) == false)
                    {
                        ViewBag.Alert = "Incorrect Email Adress";
                        return View(volunteer);
                    }
                    if (volunteerRepository.CheckVolunteerExistByPhoneOrEmail(volunteer))
                    {
                        ViewBag.Alert = "Existing Volunteer";
                        return View(volunteer);
                    }

                    volunteer.City = Helper.ValidateCity(volunteer.City);
                    volunteer.Name = Helper.ValidateName(volunteer.Name);

                    if (volunteer.ImageProfile != null)
                    {
                        if (Helper.ValidateImageProfile(volunteer.ImageProfile.Length, imageFileLengthLimit))
                        {
                            ViewBag.Alert = $"Profile image to big. Please use an image having no more than {imageFileLengthLimit} pixels.";
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
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        public IActionResult Delete(int? id)
        {
            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        /// <summary>
        /// Delete volunteer.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns></returns>
        // POST: Volunteers/Delete/5
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
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
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
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

                if (!string.IsNullOrEmpty(ToUpdatevolunteer.Phone) && Helper.PhoneNumberIsValid(ToUpdatevolunteer.Phone) == false)
                {
                    ViewBag.Phone_Error = "Incorrect phone number";
                    return View("Edit_PersonalInfo", currentUser);
                }
                if (!string.IsNullOrEmpty(ToUpdatevolunteer.InstagramProfile) && Helper.InstagramIsValid(ToUpdatevolunteer.InstagramProfile) == false)
                {
                    ViewBag.Instagram_Error = "Incorrect Instragram Profile";
                    return View("Edit_PersonalInfo", currentUser);
                }
                if (!string.IsNullOrEmpty(ToUpdatevolunteer.Email) && Helper.EmailIsValid(ToUpdatevolunteer.Email) == false)
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

                ToUpdatevolunteer.City = Helper.ValidateCity(ToUpdatevolunteer.City);
                ToUpdatevolunteer.Name = Helper.ValidateName(ToUpdatevolunteer.Name);

                if (ToUpdatevolunteer.ImageProfile != null)
                {
                    if (Helper.ValidateImageProfile(ToUpdatevolunteer.ImageProfile.Length, imageFileLengthLimit))
                    {
                        ViewBag.Image_Error = $"Profile image to big. Please use an image having no more than {imageFileLengthLimit} bytes.";
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
