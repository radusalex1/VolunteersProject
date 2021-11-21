using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Data;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    public class VolunteersController : Controller
    {
        //private readonly VolunteersContext _context;
        private IVolunteerRepository volunteerRepository;

        public VolunteersController(VolunteersContext context, IVolunteerRepository volunteerRepositoryV)
        {
            //_context = context;
            this.volunteerRepository = volunteerRepositoryV;
        }

        // GET: Volunteers
        public async Task<IActionResult> Index(
            string sortOrder,
            string SearchString,
            string currentFilter,
            int? pageNumber)
        {

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

            //var students = from s in _context.Volunteers
            //               select s;
            var students = volunteerRepository.GetVolunteers();


            if (!String.IsNullOrEmpty(SearchString))
            {
                //students = students.Where(s => s.Name.Contains(SearchString) || s.Surname.Contains(SearchString));
                students = students.Where(s => s.Name.Contains(SearchString) || s.Surname.Contains(SearchString)).ToList();
            }

            students = GetSortedVolunteers(sortOrder, students);

            int pageSize = 5;

            //return View(await PaginatedList<Volunteer>.CreateAsync(students.AsNoTracking(), pageNumber ?? 1, pageSize));
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
        //public async Task<IActionResult> Details(int? id)
        public IActionResult Details(int? id)
        {           
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var volunteer = await _context.Volunteers
            //    .Include(e => e.Enrollments)
            //    .ThenInclude(c => c.contribution)
            //         .FirstOrDefaultAsync(m => m.ID == id);
            var volunteer = volunteerRepository.GetVolunteerWithEnrollmentsById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // GET: Volunteers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Volunteers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub")] Volunteer volunteer)
        {            
            if (ModelState.IsValid)
            {
                //_context.Add(volunteer);
                //await _context.SaveChangesAsync();
                volunteerRepository.AddVolunteer(volunteer);

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

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Surname,City,BirthDate,JoinHubDate,Email,Phone,InstagramProfile,FaceBookProfile,DescriptionContributionToHub")] Volunteer volunteer)
        {
            if (id != volunteer.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //_context.Update(volunteer);
                    //await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Delete(int? id)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            //var volunteer = await _context.Volunteers
            //    .FirstOrDefaultAsync(m => m.ID == id);
            var volunteer = volunteerRepository.GetVolunteerById(id);

            if (volunteer == null)
            {
                return NotFound();
            }

            return View(volunteer);
        }

        // POST: Volunteers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //var volunteer = await _context.Volunteers.FindAsync(id);
            var volunteer = volunteerRepository.GetVolunteerById(id);

            //_context.Volunteers.Remove(volunteer);
            //await _context.SaveChangesAsync();
            volunteerRepository.DeleteVolunteer(volunteer);

            return RedirectToAction(nameof(Index));
        }        
    }
}
