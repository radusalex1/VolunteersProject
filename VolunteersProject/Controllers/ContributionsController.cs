﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Data;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    public class ContributionsController : Controller
    {
        private readonly VolunteersContext _context;
        private IVolunteerRepository repository;

        public ContributionsController(VolunteersContext context,IVolunteerRepository repository)
        {
            _context = context;
            this.repository = repository;
        }

        // GET: Contributions
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CreditsSortParam"] = sortOrder == "Credits" ? "Credits_desc" : "Credits";
            ViewData["StartDateSortParam"] = sortOrder == "sd_asc" ? "sd_desc" : "sd_asc";
            ViewData["FinishDateSortParam"] = sortOrder == "fd_asc" ? "fd_desc" : "fd_asc";


            var contributions = from c in _context.Contributions
                                select c;
            contributions = SortContributions(sortOrder, contributions);
            return View(await contributions.AsNoTracking().ToListAsync());

        }

        private static IQueryable<Contribution> SortContributions(string sortOrder, IQueryable<Contribution> contributions)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    contributions = contributions.OrderByDescending(c => c.Name);
                    break;
                case "Credits":
                    contributions = contributions.OrderBy(c => c.Credits);
                    break;
                case "Credits_desc":
                    contributions = contributions.OrderByDescending(c => c.Credits);
                    break;
                case "sd_asc":
                    contributions = contributions.OrderBy(c => c.StartDate);
                    break;
                case "sd_desc":
                    contributions = contributions.OrderByDescending(c => c.StartDate);
                    break;
                case "fd_asc":
                    contributions = contributions.OrderBy(c => c.FinishDate);
                    break;
                case "fd_desc":
                    contributions = contributions.OrderByDescending(c => c.FinishDate);
                    break;
                default:
                    contributions = contributions.OrderBy(c => c.Name);
                    break;
            }

            return contributions;
        }

        // GET: Contributions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contributions
                .FirstOrDefaultAsync(m => m.ID == id);

            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // GET: Contributions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Contributions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Credits,StartDate,FinishDate,Description")] Contribution contribution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contribution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(contribution);
        }

        // GET: Contributions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contributions.FindAsync(id);
            if (contribution == null)
            {
                return NotFound();
            }
            return View(contribution);
        }

        // POST: Contributions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Credits,StartDate,FinishDate,Description")] Contribution contribution)
        {
            if (id != contribution.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contribution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContributionExists(contribution.ID))
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
            return View(contribution);
        }

        // GET: Contributions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contributions
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // POST: Contributions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contribution = await _context.Contributions.FindAsync(id);
            _context.Contributions.Remove(contribution);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContributionExists(int id)
        {
            return _context.Contributions.Any(e => e.ID == id);
        }

        public async Task<IActionResult> Assign(int id)
        {
            var volunteers = repository.GetAvailableVolunteers(id);
            return View(volunteers);
        }
    }
}
