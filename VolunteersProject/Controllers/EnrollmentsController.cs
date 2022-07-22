using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Data;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    [Authorize]
    public class EnrollmentsController : GeneralController
    {
        private readonly VolunteersContext _context;

        private IEnrollmentRepository enrollmentRepository;
        private IVolunteerRepository volunteerRepository;
        private IContributionRepository contributionRepository;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="enrollmentRepository"></param>
        /// <param name="volunteerRepository"></param>
        /// <param name="contributionRepository"></param>
        /// <param name="logger"></param>
        /// <param name="configuration"></param>
        public EnrollmentsController(
            IUserRepository userRepository,
            IEnrollmentRepository enrollmentRepository,
            IVolunteerRepository volunteerRepository,
            IContributionRepository contributionRepository,
            ILogger<VolunteersController> logger,
            IConfiguration configuration)
                : base(logger, configuration, userRepository)
        {
            this.enrollmentRepository = enrollmentRepository;
            this.volunteerRepository = volunteerRepository;
            this.contributionRepository = contributionRepository;
        }

        // GET: Enrollments
        [Authorize(Roles = Common.Role.Admin)]
        public IActionResult Index(string SortOrder)
        {

            this.Logger.LogInformation("HttpGet EnrollmentsController Index()");

            IQueryable<Enrollment> enrollments = enrollmentRepository.GetEnrollments_With_Data();

            ViewData["NameSortParam"] = String.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            ViewData["contributionSortParam"] = SortOrder == "contr_asc" ? "contr_desc" : "contr_asc";

            enrollments = GetSortedEnrollments(SortOrder, enrollments);

            return View(enrollments);
        }

        private IQueryable<Enrollment> GetSortedEnrollments(string SortOrder, IQueryable<Enrollment> enrolments)
        {
            switch (SortOrder)
            {
                case "name_desc":
                    enrolments = enrolments.OrderByDescending(e => e.volunteer.Name);
                    break;
                case "contr_asc":
                    enrolments = enrolments.OrderBy(e => e.contribution.Name);
                    break;
                case "contr_desc":
                    enrolments = enrolments.OrderByDescending(e => e.contribution.Name);
                    break;
                default:
                    enrolments = enrolments.OrderBy(e => e.volunteer.Name);
                    break;
            }
            return enrolments;
        }

        /// <summary>
        /// get sorted elements;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var enrollment = enrollmentRepository.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            ViewData["VolunteerID"] = new SelectList(volunteerRepository.GetVolunteers(), "Id", "Id");
            ViewData["VolunteerFullName"] = new SelectList(volunteerRepository.GetVolunteers(), "Id", "FullName");
            ViewData["ContributionName"] = new SelectList(contributionRepository.GetContributions(), "Id", "Name");

            return View();
        }

        // POST: Enrollments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EnrollmentID,contributionId,VolunteerID")] Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                enrollmentRepository.Save(enrollment);

                return RedirectToAction(nameof(Index));
            }

            ViewData["VolunteerID"] = new SelectList(volunteerRepository.GetVolunteers(), "Id", "Id", enrollment.VolunteerID);

            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int id)
        {

            var enrollment = enrollmentRepository.GetEnrollmentById(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            ViewData["VolunteerID"] = new SelectList(volunteerRepository.GetVolunteers(), "Id", "Id", enrollment.VolunteerID);
            ViewData["contributionId"] = new SelectList(contributionRepository.GetContributions(), "Id", "Id", enrollment.contributionId);
            ViewData["VolunteerFullName"] = new SelectList(volunteerRepository.GetVolunteers(), "Id", "FullName", enrollment.volunteer);
            ViewData["ContributionName"] = new SelectList(contributionRepository.GetContributions(), "Id", "Name", enrollment.contribution);

            return View(enrollment);
        }

        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, [Bind("EnrollmentID,contributionId,VolunteerID")] Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    enrollmentRepository.UpdateEnrollment(enrollment);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EnrollmentExists(enrollment.EnrollmentID))
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
            ViewData["VolunteerID"] = new SelectList(volunteerRepository.GetVolunteers(), "Id", "Id", enrollment.VolunteerID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = enrollmentRepository.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = enrollmentRepository.GetEnrollmentById(id);
            enrollmentRepository.DeleteEnrollment(enrollment);


            return RedirectToAction(nameof(Index));
        }

        // GET: Enrollments/VolunteerEmailAnswer/5/1
        public ActionResult VolunteerEmailAnswer(int contributionId, int volunteerId)
        {
            var contribution = contributionRepository.GetContributionById(contributionId);

            var volunteer = volunteerRepository.GetVolunteerById(volunteerId);

            if (contribution == null || volunteer == null)
            {
                return NotFound();
            }

            var volunteerEmailAnswer = new VolunteerEmailAnswerModel
            {
                ContributionId = contribution.Id,
                ContributionName = contribution.Name,
                StartDate = contribution.StartDate.ToString("yyyy-MM-dd"),
                FinishDate = contribution.FinishDate.ToString("yyyy-MM-dd"),
                VolunteerId = volunteer.Id,
            };

            return View(volunteerEmailAnswer);
        }

        [HttpPost]
        public ActionResult SaveVolunteerEmailAnswer(IFormCollection form, int contributionId, int volunteerId)
        {
            var volunteerEnrollmentStatus = -1;

            if (!string.IsNullOrEmpty(form["Accept"]))
            {
                volunteerEnrollmentStatus = 2;
            }
            else if (!string.IsNullOrEmpty(form["Decline"]))
            {
                volunteerEnrollmentStatus = 3;
            }

            var enrollment = new Enrollment
            {
                contributionId = contributionId,
                VolunteerID = volunteerId,
                VolunteerStatus = volunteerEnrollmentStatus
            };

            enrollmentRepository.UpdateEnrollment(enrollment);

            return View("Close");
        }

        private bool EnrollmentExists(int id)
        {
            Enrollment enrollment = enrollmentRepository.GetEnrollmentById(id);
            return enrollmentRepository.EnrollmentExists(enrollment);

        }
    }
}
