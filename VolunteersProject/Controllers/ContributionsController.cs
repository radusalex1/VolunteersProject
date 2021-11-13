using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Data;
using VolunteersProject.Email;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    public class ContributionsController : Controller
    {
        private readonly VolunteersContext _context;
        private IVolunteerRepository repository;
        private IEmailService emailService;

        public ContributionsController(VolunteersContext context, IVolunteerRepository repository, IEmailService emailService)
        {
            _context = context;
            this.repository = repository;
            this.emailService = emailService;
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
            //var contributions = (IQueryable<Contribution>)this.repository.GetContributions();

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

            var selectedVolunteers = new SelectedVolunters
            {
                ContributionId = id,
                Volunteers = new List<Volunteer>()
            };
            selectedVolunteers.Volunteers.AddRange(volunteers);

            return View(selectedVolunteers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(IFormCollection form, int contributionId)
        {
            //todo cia - este urat ce am scris
            var volunteers = repository.GetAvailableVolunteers(Convert.ToInt32(contributionId));

            var sendInvitationEmailList = new List<Volunteer>();
            foreach (var volunteer in volunteers)
            {
                if (!string.IsNullOrEmpty(form["chk_emailInvitation_" + volunteer.ID]))
                {
                    if (form["chk_emailInvitation_" + volunteer.ID][0] == "true")
                    {
                        volunteer.IsSelected = true;
                        sendInvitationEmailList.Add(volunteer);
                    }
                    //else
                    //{
                    //    volunteer.IsSelected = false;
                    //}
                }
            }

            SendEmail(sendInvitationEmailList);



            //todo cia - check directly assigned checkbox (directAssignmentChkBx) - if checked then: 1.save in DB and a new item should appears in Volunteers*Events; 2. the selection should dissapear from this list;

            var directAssignmentVolunteerList = new List<Volunteer>();
            foreach (var volunteer in volunteers)
            {
                if (!string.IsNullOrEmpty(form["chk_directAssignment_" + volunteer.ID]))
                {
                    if (form["chk_directAssignment_" + volunteer.ID][0] == "true")
                    {
                        directAssignmentVolunteerList.Add(volunteer);
                    }
                }
            }

            //Insert in Volunteers*Events tbl the directAssignmentVolunteerList


            //todo cia - ce urmeaza este doar provizoriu ca sa nu dea eroare

            //todo cia - salveaza in baza selectia de useri (la ei s-a trimis doar email) si call SendEmail(...)           


            var selectedVolunteers = new SelectedVolunters
            {
                ContributionId = contributionId,
                Volunteers = new List<Volunteer>()
            };
            selectedVolunteers.Volunteers.AddRange(volunteers);



            return View(selectedVolunteers);
        }

        public void SendEmail(List<Volunteer> sendInvitationEmailList)
        {
            //var volunteers = repository.GetVolunteers();
            ////return View(volunteers);
            //Console.WriteLine("am ajuns");

            foreach (var volunteer in sendInvitationEmailList)
            {
                var emailMessage = new EmailMessage();

                emailMessage.Subject = $"this is an email test for {volunteer.FullName} having email {volunteer.Email}";

                emailMessage.ToAddresses = new List<EmailAddress>
                {
                    new EmailAddress { Address = "ciprian_alexandru@hotmail.com" }
                };

                emailMessage.Content = "your where invited to happy camps";


                emailMessage.FromAddresses = new List<EmailAddress>
                {
                    new EmailAddress { Address = "codruta_alexandru@hotmail.com" }
                };

                emailService.Send(emailMessage);
            }            
        }       
    }
}
