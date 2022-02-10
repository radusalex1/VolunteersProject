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
    public class ContributionsController : GeneralConstroller
    {

        private IVolunteerRepository volunteerRepository;
        private IEmailService emailService;
        private IEnrollmentRepository enrollmentRepository;
        private IContributionRepository contributionRepository;

        public ContributionsController
             (
                 ILogger<ContributionsController> logger,
                 VolunteersContext context,
                 IVolunteerRepository volunteerRepository,
                 IEmailService emailService,
                 IEnrollmentRepository enrollmentRepository,
                 IContributionRepository contributionRepository,
                 IConfiguration configuration
             ) : base(logger, configuration)
        {
            this.volunteerRepository = volunteerRepository;
            this.emailService = emailService;
            this.enrollmentRepository = enrollmentRepository;
            this.contributionRepository = contributionRepository;
        }

        // GET: Contributions
        public async Task<IActionResult> Index(string sortOrder)
        {
            this.Logger.LogInformation("HttpGet ContributionsController Index()");

            var contributions = new List<Contribution>();

            try
            {
                ViewData["NameSortParam"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
                ViewData["CreditsSortParam"] = sortOrder == "Credits" ? "Credits_desc" : "Credits";
                ViewData["StartDateSortParam"] = sortOrder == "sd_asc" ? "sd_desc" : "sd_asc";
                ViewData["FinishDateSortParam"] = sortOrder == "fd_asc" ? "fd_desc" : "fd_asc";

                contributions = contributionRepository.GetContributions();

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

        

        // GET: Contributions/Details/5
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> Details(int id)
        {


            var contribution = contributionRepository.GetContributionById(id);
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // GET: Contributions/Create
        [Authorize(Roles = Common.Role.Admin)]
        public IActionResult Create()
        {
            var contribution = new Contribution
            {
                StartDate = DateTime.Today,
                FinishDate = DateTime.Today,
                VolunteerDeadlineConfirmation = DateTime.Today
            };

            return View(contribution);
        }

        // POST: Contributions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> Create([Bind("Id,Name,Credits,StartDate,FinishDate,Description,VolunteerDeadlineConfirmation")] Contribution contribution)
        {
            if (ModelState.IsValid)
            {
               
                if(string.IsNullOrEmpty(contribution.Name))
                {
                    ViewBag.Contribution_Name_Err = "Name cannot be empty!";
                    return View(contribution);
                }

                if(contribution.Credits<=0)
                {
                    ViewBag.Credits_Err = "Invalid input!";
                    return View(contribution);

                }

                if (contribution.StartDate > contribution.FinishDate)
                {
                    ViewBag.Dates_Validation_Err = "Start Date > Finish Date!";
                    return View(contribution);
                }

                if(contribution.VolunteerDeadlineConfirmation>contribution.StartDate)
                {
                    ViewBag.VolunteerDeadlineConfirmation_Error = "This date cannot be after Start Date!";
                    return View(contribution);
                }

                contributionRepository.AddContribution(contribution);
                return RedirectToAction(nameof(Index));
            }
           
            return View(contribution);
        }

        // GET: Contributions/Edit/5
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = contributionRepository.GetContributionById(id);

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
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Credits,StartDate,FinishDate,Description,VolunteerDeadlineConfirmation")] Contribution contribution)
        {
          
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(contribution.Name))
                    {
                        ViewBag.Contribution_Name_Err = "Name cannot be empty!";
                        return View(contribution);
                    }

                    if (contribution.Credits <= 0)
                    {
                        ViewBag.Credits_Err = "Invalid input!";
                        return View(contribution);

                    }

                    if (contribution.StartDate > contribution.FinishDate)
                    {
                        ViewBag.Dates_Validation_Err = "Start Date > Finish Date!";
                        return View(contribution);
                    }

                    if (contribution.VolunteerDeadlineConfirmation > contribution.StartDate)
                    {
                        ViewBag.VolunteerDeadlineConfirmation_Error = "This date cannot be after Start Date!";
                        return View(contribution);
                    }
                    contributionRepository.UpdateContribution(contribution);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (contributionRepository.ContributionExists(contribution.Id))
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
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = contributionRepository.GetContributionById(id);
            
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // POST: Contributions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contribution = contributionRepository.GetContributionById(id);
            contributionRepository.DeleteContribution(contribution);
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = Common.Role.Admin)]
        public async Task<IActionResult> Assign(int id)
        {
            var selectedVolunteers = GetAvailableVolunteersDTO(id);

            return View(selectedVolunteers);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Common.Role.Admin)]
        public ActionResult Assign(IFormCollection form, int contributionId)
        {            
            var selectedVolunteers = GetAvailableVolunteersDTO(contributionId);
            
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
                enrollment.VolunteerID = selectedVolunteer.Id;
                enrollment.VolunteerStatus = (int)VolunteerEnrollmentStatusEnum.Pending;
                enrollmentRepository.Save(enrollment);
            }
        }

        private List<VolunteerDTO> GetSelectedVolunteersForDirectAssignment(IFormCollection form, List<VolunteerDTO> availableVolunteers)
        {
            var directAssignmentVolunteerList = new List<VolunteerDTO>();

            foreach (var volunteer in availableVolunteers)
            {
                if (!string.IsNullOrEmpty(form["chk_directAssignment_" + volunteer.Id]))
                {
                    if (form["chk_directAssignment_" + volunteer.Id][0] == "true")
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
                if (!string.IsNullOrEmpty(form["chk_emailInvitation_" + volunteer.Id]))
                {
                    if (form["chk_emailInvitation_" + volunteer.Id][0] == "true")
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
        [Authorize(Roles = Common.Role.Admin)]
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

                var contribution = contributionRepository.GetContributionById(contributionId);

                var link = GetLink(contributionId, volunteer.Id);

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

            var applicationSiteName = configuration.GetSection("AppSettings").GetSection("ApplicationSiteName").Value;

            var action = $"{applicationSiteName}/Enrollments/VolunteerEmailAnswer?contributionId={contributionId}&volunteerId={volunteerId}";

            return $"<a href=\"{server}{action}\">link</a>";
        }

        /// <summary>
        /// Save assignment of volunteers to a specific contribution.
        /// </summary>
        /// <param name="contributionId">Contribution id.</param>
        /// <param name="directAssignmentVolunteerList">Selected volunteer list.</param>
        [Authorize(Roles = Common.Role.Admin)]
        public void SaveDirectAssignedVoluteersToContribution(int contributionId, List<VolunteerDTO> directAssignmentVolunteerList)
        {
            foreach (var selectedVolunteer in directAssignmentVolunteerList)
            {
                var enrollment = new Enrollment();

                enrollment.contributionId = contributionId;
                enrollment.VolunteerID = selectedVolunteer.Id;
                enrollment.VolunteerStatus = (int)VolunteerEnrollmentStatusEnum.Accepted;
                enrollmentRepository.Save(enrollment);
            }
        }

        private AvailableVolunters GetAvailableVolunteersDTO(int id)
        {
            //todo cia - this and the next method should be refactor
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
                    Id = volunteerItem.Id,
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
