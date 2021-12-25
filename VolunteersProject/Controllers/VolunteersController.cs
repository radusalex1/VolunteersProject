using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using VolunteersProject.Common;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Volunteer controller.
    /// </summary>
    [Authorize]
    public class VolunteersController : Controller
    {
        //private readonly VolunteersContext _context;
        private readonly ILogger<VolunteersController> logger;
        private IVolunteerRepository volunteerRepository;


        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="volunteerRepository">Volunteer repository.</param>
        public VolunteersController(ILogger<VolunteersController> logger, IVolunteerRepository volunteerRepository)
        {
            this.logger = logger;
            this.volunteerRepository = volunteerRepository;
        }


        // GET: Volunteers
        [Authorize(Roles = "User, Admin")]
        public async Task<IActionResult> Index(
            string sortOrder,
            string SearchString,
            string currentFilter,
            int? pageNumber)
        {
            this.logger.LogInformation("HttpGet VolunteersContr Index()");

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
                students = students.Where(s => s.Name.Contains(SearchString) || s.Surname.Contains(SearchString)).ToList();
            }

            students = GetSortedVolunteers(sortOrder, students);

            ///todo Radu - read from appconfig
            int pageSize = 5;

            return View(PaginatedList<Volunteer>.Create(students, pageNumber ?? 1, pageSize));
        }

        //private IQueryable<Volunteer> GetSortedVolunteers(string sortOrder, IQueryable<Volunteer> students)
        private List<Volunteer> GetSortedVolunteers(string sortOrder, List<Volunteer> students)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    //students = students.OrderByDescending(s => s.Name);
                    students = students.OrderByDescending(s => s.Name).ToList();
                    break;
                case "Age":
                    //students = students.OrderBy(s => s.BirthDate);
                    students = students.OrderBy(s => s.BirthDate).ToList();
                    break;
                case "Age_desc":
                    //students = students.OrderByDescending(s => s.BirthDate);
                    students = students.OrderByDescending(s => s.BirthDate).ToList();
                    break;
                case "City_desc":
                    //students = students.OrderByDescending(s => s.City);
                    students = students.OrderByDescending(s => s.City).ToList();
                    break;
                case "City":
                    //students = students.OrderBy(s => s.City);
                    students = students.OrderBy(s => s.City).ToList();
                    break;
                case "JoinHubDate":
                    //students = students.OrderBy(s => s.JoinHubDate);
                    students = students.OrderBy(s => s.JoinHubDate).ToList();
                    break;
                case "JoinHubDate_desc":
                    //students = students.OrderByDescending(s => s.JoinHubDate);
                    students = students.OrderByDescending(s => s.JoinHubDate).ToList();
                    break;
                default:
                    //students = students.OrderBy(s => s.Name);
                    students = students.OrderBy(s => s.Name).ToList();
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
        public IActionResult Details(int? id)
        {
            var volunteer = volunteerRepository.GetVolunteerWithEnrollmentsById(id);

            if (volunteer.ImageProfileByteArray != null)
            {
                ViewBag.Image = ViewImage(volunteer.ImageProfileByteArray);
            }

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        [Authorize(Roles = Role.Admin)]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Create([Bind("ID,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub,ImageProfile")] Volunteer volunteer)
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
                if (volunteerRepository.VolunteerExists(volunteer))
                {
                    ViewBag.Alert = "Existing Volunteer";
                    return View(volunteer);
                }

                volunteer.City = validateCity(volunteer.City);
                volunteer.Name = validateName(volunteer.Name);


                if (volunteer.ImageProfile != null)
                {
                    //var img = volunteer.ImageProfile;

                    //var fileName = Path.GetFileName(volunteer.ImageProfile.FileName);
                    //var contentType = volunteer.ImageProfile.ContentType;

                    volunteer.ImageProfileByteArray = GetByteArrayFromImage(volunteer.ImageProfile);
                }


                volunteerRepository.AddVolunteer(volunteer);

                ViewBag.Alert = "Volunteer added successfully";

                return RedirectToAction(nameof(Index));
            }
            return View(volunteer);
        }

        // GET: Volunteers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer.ImageProfileByteArray != null)
            {
                ViewBag.Image = ViewImage(volunteer.ImageProfileByteArray);
            }

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub,ImageProfile")] Volunteer volunteer)
        {
            if (id != volunteer.ID)
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
                    if (volunteerRepository.VolunteerExists(volunteer))
                    {
                        ViewBag.Alert = "Existing Volunteer";
                        return View(volunteer);
                    }

                    volunteer.City = validateCity(volunteer.City);
                    volunteer.Name = validateName(volunteer.Name);


                    if (volunteer.ImageProfile != null)
                    {
                        volunteer.ImageProfileByteArray = GetByteArrayFromImage(volunteer.ImageProfile);
                    }

                    volunteerRepository.UpdateVolunteer(volunteer);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!volunteerRepository.VolunteerExists(volunteer.ID))
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

        // GET: Volunteers/Delete/5
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> Delete(int? id)
        {

            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [Authorize(Roles = Role.Admin)]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var volunteer = volunteerRepository.GetVolunteerById(id);

            volunteerRepository.DeleteVolunteer(volunteer);

            return RedirectToAction(nameof(Index));
        }

        private byte[] GetByteArrayFromImage(IFormFile file)
        {
            using (var target = new MemoryStream())
            {
                file.CopyTo(target);
                return target.ToArray();
            }
        }

        private string validateCity(string city)
        {
            return char.ToUpper(city[0]) + city.Substring(1);
        }

        private string validateName(string name)
        {
            name = name.ToUpper();
            return name;
        }

        private bool PhoneNumberIsValid(string phoneNumber)
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

        private bool InstagramIsValid(string instagramProfile)
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

        private bool EmailIsValid(string email)
        {
            if (email.Trim().EndsWith("."))
            {
                return false; // suggested by @TK-421
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

        [NonAction]
        private string ViewImage(byte[] arrayImage)
        {
            string base64String = Convert.ToBase64String(arrayImage, 0, arrayImage.Length);

            return "data:image/png;base64," + base64String;
        }
    }
}
