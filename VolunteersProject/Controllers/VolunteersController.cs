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
    public class VolunteersController : GeneralConstroller
    {
        public IVolunteerRepository volunteerRepository;
        private int pageSize;
        private int imgWidth;
        private int imgHeight;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="volunteerRepository">Volunteer repository.</param>
        /// <param name="configuration">Application configuration.</param>
        public VolunteersController(ILogger<VolunteersController> logger, IConfiguration configuration, IVolunteerRepository volunteerRepository) : base(logger, configuration)
        {
            this.volunteerRepository = volunteerRepository;
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


            var students = volunteerRepository.GetVolunteers();


            if (!String.IsNullOrEmpty(SearchString))
            {
                students = students.Where(s => s.Name.Contains(SearchString) || s.Surname.Contains(SearchString));
            }

            students = GetSortedVolunteers(sortOrder, students);

            return View(PaginatedList<Volunteer>.Create(students, pageNumber ?? 1, pageSize));
        }

        private IQueryable<Volunteer> GetSortedVolunteers(string sortOrder, IQueryable<Volunteer> students)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    students = students.OrderByDescending(s => s.Name);
                    break;
                case "Age":
                    students = students.OrderBy(s => s.BirthDate);
                    break;
                case "Age_desc":
                    students = students.OrderByDescending(s => s.BirthDate);
                    break;
                case "City_desc":
                    students = students.OrderByDescending(s => s.City);
                    break;
                case "City":
                    students = students.OrderBy(s => s.City);
                    break;
                case "JoinHubDate":
                    students = students.OrderBy(s => s.JoinHubDate);
                    break;
                case "JoinHubDate_desc":
                    students = students.OrderByDescending(s => s.JoinHubDate);
                    break;
                default:
                    students = students.OrderBy(s => s.Name);
                    break;
            }

            return students;
        }

        /// <summary>
        /// Here sort Students
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
    }
}
