using MailServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Data;
using VolunteersProject.Models;
using VolunteersProject.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using VolunteersProject.Common;
using Microsoft.Extensions.Logging;
using VolunteersProject.DTO;

namespace VolunteersProject.Controllers
{
    [Authorize]
    public class ContributionsController : Controller
    {
        private readonly VolunteersContext _context;
        private readonly ILogger<ContributionsController> logger;
        private IVolunteerRepository volunteerRepository;
        private IEmailService emailService;
        private IEnrollmentRepository enrollmentRepository;
        private IContributionRepository contributionRepositor;
        private IConfiguration configuration;

        public ContributionsController
            (
                ILogger<ContributionsController> logger,
                VolunteersContext context,
                IVolunteerRepository volunteerRepository,
                IEmailService emailService,
                IEnrollmentRepository enrollmentRepository,
                IContributionRepository contributionRepositor,
                IConfiguration configuration
            )
        {
            _context = context;
            this.logger = logger;
            this.volunteerRepository = volunteerRepository;
            this.emailService = emailService;
            this.enrollmentRepository = enrollmentRepository;
            this.contributionRepositor = contributionRepositor;
            this.configuration = configuration;
        }

        // GET: Contributions
        public async Task<IActionResult> Index(string sortOrder)
        {
            this.logger.LogInformation("HttpGet ContributionsController Index()");

            var contributions = new List<Contribution>();

            try
            {
                ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewData["CreditsSortParam"] = sortOrder == "Credits" ? "Credits_desc" : "Credits";
                ViewData["StartDateSortParam"] = sortOrder == "sd_asc" ? "sd_desc" : "sd_asc";
                ViewData["FinishDateSortParam"] = sortOrder == "fd_asc" ? "fd_desc" : "fd_asc";

                contributions = contributionRepositor.GetContributions();

                if (contributions == null)
                {

                    return RedirectToAction("Error", "Account", new { errorMessage = "Contribution is null." });
                }

                contributions = SortContributions(sortOrder, contributions);

                if (contributions == null)
                {
                    return RedirectToAction("Error", "Account", new { errorMessage = "SortContributions returns null." });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", "Account", new { errorMessage = ex.Message });
            }

            return View(contributions);
        }

        private static List<Contribution> SortContributions(string sortOrder, List<Contribution> contributions)
        {
            switch (sortOrder)
            {
                case "name_desc":
                    contributions = contributions.OrderByDescending(c => c.Name).ToList();
                    break;
                case "Credits":
                    contributions = contributions.OrderBy(c => c.Credits).ToList();
                    break;
                case "Credits_desc":
                    contributions = contributions.OrderByDescending(c => c.Credits).ToList();
                    break;
                case "sd_asc":
                    contributions = contributions.OrderBy(c => c.StartDate).ToList();
                    break;
                case "sd_desc":
                    contributions = contributions.OrderByDescending(c => c.StartDate).ToList();
                    break;
                case "fd_asc":
                    contributions = contributions.OrderBy(c => c.FinishDate).ToList();
                    break;
                case "fd_desc":
                    contributions = contributions.OrderByDescending(c => c.FinishDate).ToList();
                    break;
                default:
                    contributions = contributions.OrderBy(c => c.Name).ToList();
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

            var contribution = await _context.Contributions.FirstOrDefaultAsync(m => m.ID == id);

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
        public async Task<IActionResult> Create([Bind("ID,Name,Credits,StartDate,FinishDate,Description,VolunteerDeadlineConfirmation")] Contribution contribution)
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
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Credits,StartDate,FinishDate,Description,VolunteerDeadlineConfirmation")] Contribution contribution)
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
            var selectedVolunteers = GetAvailableVolunteersDTO(id);

            return View(selectedVolunteers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(IFormCollection form, int contributionId)
        {
            //var availableVolunteers = volunteerRepository.GetAvailableVolunteers(Convert.ToInt32(contributionId));
            var selectedVolunteers = GetAvailableVolunteersDTO(contributionId);

            //var sendInvitationEmailList = GetSelectedVolunteersForSendEmail(form, availableVolunteers);
            var sendInvitationEmailList = GetSelectedVolunteersForSendEmail(form, selectedVolunteers.VolunteersDTO);


            UpdateToPending(contributionId, sendInvitationEmailList);
            SendEmail(contributionId, sendInvitationEmailList);

            var directAssignmentVolunteerList = GetSelectedVolunteersForDirectAssignment(form, selectedVolunteers.VolunteersDTO);
            SaveDirectAssignedVoluteersToContribution(contributionId, directAssignmentVolunteerList);


            //reload evailable list
            selectedVolunteers = GetAvailableVolunteersDTO(contributionId);

            return View(selectedVolunteers);
        }

        private void UpdateToPending(int contributionId, List<VolunteerDTO> sendInvitationEmailList)
        {
            foreach (var selectedVolunteer in sendInvitationEmailList)
            {
                var enrollment = new Enrollment();

                enrollment.contributionId = contributionId;
                enrollment.VolunteerID = selectedVolunteer.ID;
                enrollment.VolunteerStatus = (int)VolunteerEnrollmentStatusEnum.Pending;
                enrollmentRepository.Save(enrollment);
            }
        }

        private List<VolunteerDTO> GetSelectedVolunteersForDirectAssignment(IFormCollection form, List<VolunteerDTO> availableVolunteers)
        {
            var directAssignmentVolunteerList = new List<VolunteerDTO>();

            foreach (var volunteer in availableVolunteers)
            {
                if (!string.IsNullOrEmpty(form["chk_directAssignment_" + volunteer.ID]))
                {
                    if (form["chk_directAssignment_" + volunteer.ID][0] == "true")
                    {
                        directAssignmentVolunteerList.Add(volunteer);
                    }
                }
            }

            return directAssignmentVolunteerList;
        }

        private List<VolunteerDTO> GetSelectedVolunteersForSendEmail(IFormCollection form, List<VolunteerDTO> availableVolunteers)
        {
            var sendInvitationEmailList = new List<VolunteerDTO>();

            foreach (var volunteer in availableVolunteers)
            {
                if (!string.IsNullOrEmpty(form["chk_emailInvitation_" + volunteer.ID]))
                {
                    if (form["chk_emailInvitation_" + volunteer.ID][0] == "true")
                    {
                        volunteer.IsSelected = true;
                        sendInvitationEmailList.Add(volunteer);
                    }
                }
            }

            return sendInvitationEmailList;
        }

        /// <summary>
        /// Send email to selected volunteers
        /// </summary>
        /// <param name="sendInvitationEmailList">Selected volunteer list.</param>
        public void SendEmail(int contributionId, List<VolunteerDTO> sendInvitationEmailList)
        {
            foreach (var volunteer in sendInvitationEmailList)
            {
                var emailMessage = new EmailMessage();

                emailMessage.Subject = $"This is an email for {volunteer.FullName} having email {volunteer.Email}";

                emailMessage.ToAddresses = new List<EmailAddress>
                {
                    new EmailAddress { Address = volunteer.Email }
                };

                var contribution = contributionRepositor.GetContributionById(contributionId);

                var link = GetLink(contributionId, volunteer.ID);

                emailMessage.Content = $"You where invited to \"{contribution.Name}\", a HappyCamps activitity, that will take place between {contribution.StartDate.ToString("yyyy-MM-dd")} and {contribution.FinishDate.ToString("yyyy-MM-dd")}. " +
               $"Click on {link} for more details.";

                var emailSender = configuration.GetSection("AppSettings").GetSection("EmailSender").Value;
                emailMessage.FromAddresses = new List<EmailAddress>
                {
                    new EmailAddress { Address = emailSender }
                };

                emailService.Send(emailMessage);
            }
        }

        private string GetLink(int contributionId, int volunteerId)
        {
            var server = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}";

            var action = $"/Enrollments/VolunteerEmailAnswer?contributionId={contributionId}&volunteerId={volunteerId}";

            return $"<a href=\"{ server}{ action}\">link</a>";
        }

        /// <summary>
        /// Save assignment of volunteers to a specific contribution.
        /// </summary>
        /// <param name="contributionId">Contribution id.</param>
        /// <param name="directAssignmentVolunteerList">Selected volunteer list.</param>
        public void SaveDirectAssignedVoluteersToContribution(int contributionId, List<VolunteerDTO> directAssignmentVolunteerList)
        {
            foreach (var selectedVolunteer in directAssignmentVolunteerList)
            {
                var enrollment = new Enrollment();

                enrollment.contributionId = contributionId;
                enrollment.VolunteerID = selectedVolunteer.ID;
                enrollment.VolunteerStatus = (int)VolunteerEnrollmentStatusEnum.Accepted;
                enrollmentRepository.Save(enrollment);
            }
        }

        private AvailableVolunters GetAvailableVolunteersDTO(int id)
        {
            var volunteersDTO = GetVolunteersDTO(id);

            var selectedVolunteers = new AvailableVolunters
            {
                ContributionId = id,
                VolunteersDTO = new List<VolunteerDTO>()
            };

            selectedVolunteers.VolunteersDTO.AddRange(volunteersDTO);

            return selectedVolunteers;
        }

        private List<VolunteerDTO> GetVolunteersDTO(int id)
        {
            var volunteers = volunteerRepository.GetAvailableVolunteers(id);

            var volunteersDTO = new List<VolunteerDTO>();

            foreach (var volunteerItem in volunteers)
            {
                var volunteerDTO = new VolunteerDTO
                {
                    ID = volunteerItem.ID,
                    Phone = volunteerItem.Phone,
                    Email = volunteerItem.Email,
                    FullName = volunteerItem.FullName,
                    JoinHubDate = volunteerItem.JoinHubDate
                };

                volunteersDTO.Add(volunteerDTO);
            }

            return volunteersDTO;
        }
    }
}
