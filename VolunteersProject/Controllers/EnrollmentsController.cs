using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
    public class EnrollmentsController : Controller
    {
        private readonly VolunteersContext _context;

        private readonly ILogger<EnrollmentsController> logger;
        private IEnrollmentRepository enrollmentRepository;
        private IVolunteerRepository volunteerRepository;
        private IContributionRepository contributionRepositor;

        public EnrollmentsController(VolunteersContext context, ILogger<EnrollmentsController> logger, IEnrollmentRepository enrollmentRepository, IContributionRepository contributionRepositor, IVolunteerRepository volunteerRepository)
        {
            _context = context;
            this.logger = logger;
            this.enrollmentRepository = enrollmentRepository;
            this.volunteerRepository = volunteerRepository;
            this.contributionRepositor = contributionRepositor;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(string SortOrder)
        {
            this.logger.LogInformation("HttpGet EnrollmentsController Index()");

            var volunteersContext = _context.Enrollments.Include(e => e.volunteer).Include(e => e.contribution).OrderBy(c => c.contribution.Name);

            ViewData["NameSortParam"] = String.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            ViewData["contributionSortParam"] = SortOrder == "contr_asc" ? "contr_desc" : "contr_asc";

            var enrolments = from e in volunteersContext
                             select e;

            enrolments = GetSortedEnrollments(SortOrder, enrolments);

            return View(enrolments);
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
        }/// <summary>
         /// get sorted elements;
         /// </summary>
         /// <param name="id"></param>
         /// <returns></returns>

        // GET: Enrollments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(v => v.volunteer)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: Enrollments/Create
        public IActionResult Create()
        {
            //todo cia - fill ViewData below only with not assigned data - first select a contribution and after that display only the not already assigned volunteers
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "ID", "ID");
            ViewData["VolunteerFullName"] = new SelectList(_context.Volunteers, "ID", "FullName");
            ViewData["ContributionName"] = new SelectList(_context.Contributions, "ID", "Name");

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

            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "ID", "ID", enrollment.VolunteerID);

            return View(enrollment);
        }

        // GET: Enrollments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "ID", "ID", enrollment.VolunteerID);
            ViewData["contributionId"] = new SelectList(_context.Contributions, "ID", "ID", enrollment.contributionId);
            ViewData["VolunteerFullName"] = new SelectList(_context.Volunteers, "ID", "FullName", enrollment.volunteer);
            ViewData["ContributionName"] = new SelectList(_context.Contributions, "ID", "Name", enrollment.contribution);

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
                    //_context.Update(enrollment);
                    //await _context.SaveChangesAsync();
                    enrollmentRepository.Update(enrollment);
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
            ViewData["VolunteerID"] = new SelectList(_context.Volunteers, "ID", "ID", enrollment.VolunteerID);
            return View(enrollment);
        }

        // GET: Enrollments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.volunteer)
                .FirstOrDefaultAsync(m => m.EnrollmentID == id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Enrollments/VolunteerEmailAnswer/5/1
        public ActionResult VolunteerEmailAnswer(int contributionId , int volunteerId)
        {          
            var contribution = contributionRepositor.GetContributionById(contributionId);

            var volunteer = volunteerRepository.GetVolunteerById(volunteerId);

            if (contribution == null || volunteer == null)
            {
                return NotFound();
            }

            var volunteerEmailAnswer = new VolunteerEmailAnswerModel
            {
                ContributionId = contribution.ID,
                ContributionName = contribution.Name,
                StartDate = contribution.StartDate.ToString("yyyy-MM-dd"),
                FinishDate = contribution.FinishDate.ToString("yyyy-MM-dd"),
                VolunteerId = volunteer.ID,
            };

            return View(volunteerEmailAnswer);
        }
        
        [HttpPost]
        public ActionResult SaveVolunteerEmailAnswer(IFormCollection form, int contributionId, int volunteerId)
        {
            var volunteerEnrollmentStatus=-1;

            if (!string.IsNullOrEmpty(form["Accept"]))
            {
                volunteerEnrollmentStatus = 2;
            }
            else if (!string.IsNullOrEmpty(form["Decline"]))
            {
                volunteerEnrollmentStatus = 3;
            }

            //todo Radu - implement volunteerEnrollmentStatus column in Enrollment tbl (column type = integer)

            var enrollment = new Enrollment
            {
                contributionId = contributionId,
                VolunteerID = volunteerId,
                VolunteerStatus = (int)volunteerEnrollmentStatus
            };

            enrollmentRepository.Update(enrollment);

            return View("Close");
        }

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }
    }
}
