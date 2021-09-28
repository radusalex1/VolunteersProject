using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly VolunteersContext _context;

        public EnrollmentsController(VolunteersContext context)
        {
            _context = context;
        }

        // GET: Enrollments
        public async Task<IActionResult> Index(string SortOrder)
        {
            var volunteersContext = _context.Enrollments.Include(e => e.volunteer).Include(e => e.contribution);

            //ViewData["NameSortParam"] = String.IsNullOrEmpty(SortOrder) ? "name_desc" : "";
            //ViewData["contributionSortParam"] = SortOrder == "contr_asc" ? "contr_desc" : "contr_asc";
            //var enrolments = from e in volunteersContext
            //                 select e;
            //switch(SortOrder)
            //{
            //    case "name_desc":
            //        enrolments = enrolments.OrderByDescending(e => e.volunteer.Name);
            //        break;
            //    case "contr_asc":
            //        enrolments = enrolments.OrderBy(e => e.contribution.Name);
            //        break;
            //    case "contr_desc":
            //        enrolments = enrolments.OrderByDescending(e => e.contribution.Name);
            //        break;
            //    default:
            //        enrolments = enrolments.OrderBy(e => e.volunteer.Name);
            //        break;
            //}
            ////return View(await enrolments.AsNoTracking().ToListAsync);
            return View(await volunteersContext.AsNoTracking().ToListAsync());
        }

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
                _context.Add(enrollment);
                await _context.SaveChangesAsync();
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
            ViewData["VolunteerFullName"] = new SelectList(_context.Volunteers, "ID", "FullName",enrollment.volunteer);
            ViewData["ContributionName"] = new SelectList(_context.Contributions, "ID", "Name",enrollment.contribution);

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
                    _context.Update(enrollment);
                    await _context.SaveChangesAsync();
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

        private bool EnrollmentExists(int id)
        {
            return _context.Enrollments.Any(e => e.EnrollmentID == id);
        }
    }
}
