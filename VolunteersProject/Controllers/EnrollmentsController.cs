﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using VolunteersProject.Common;
using VolunteersProject.Data;
using VolunteersProject.Filters;
using VolunteersProject.Models;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{
    /// <summary>
    /// Enrollments class.
    /// </summary>
    public class EnrollmentsController : GeneralController
    {
        private IEnrollmentRepository enrollmentRepository;
        private IVolunteerRepository volunteerRepository;
        private IContributionRepository contributionRepository;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userRepository">User repository.</param>
        /// <param name="enrollmentRepository">Enrollment repository.</param>
        /// <param name="volunteerRepository">Volunteer repository.</param>
        /// <param name="contributionRepository">Contribution repository.</param>
        /// <param name="logger">Logger.</param>
        /// <param name="configuration">Configuration.</param>
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

        /// <summary>
        /// Get a list of enrollments.
        /// </summary>
        /// <param name="SortOrder">Sort order.</param>
        /// <returns>Return list of enrollments.</returns>
        // GET: Enrollments
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.User)]
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
        /// Display selected enrollment.
        /// </summary>
        /// <param name="id">Enrollment id.</param>
        /// <returns>Display selected enrollment.</returns>
        // GET: Enrollments/Details/5
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.User)]
        public IActionResult Details(int id)
        {
            var enrollment = enrollmentRepository.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        /// <summary>
        /// Edit enrollment.
        /// </summary>
        /// <param name="id">Enrollment id.</param>
        /// <returns>Edit enrollment.</returns>
        // GET: Enrollments/Edit/5
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        public IActionResult Edit(int id)
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

        /// <summary>
        /// Edit enrollment.
        /// </summary>
        /// <param name="id">Enrollment id.</param>
        /// <param name="enrollment">Enrollment.</param>
        /// <returns>Edit enrollment.</returns>
        // POST: Enrollments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        public ActionResult Edit(int id, [Bind("EnrollmentID,contributionId,VolunteerID")] Enrollment enrollment)
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

        /// <summary>
        /// Delete enrollment.
        /// </summary>
        /// <param name="id">Enrollment id.</param>
        /// <returns>Delete enrollment.</returns>
        // GET: Enrollments/Delete/5
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        public IActionResult Delete(int id)
        {
            var enrollment = enrollmentRepository.GetEnrollmentById(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        /// <summary>
        /// Delete enrollment.
        /// </summary>
        /// <param name="id">Enrollment id.</param>
        /// <returns>Delete enrollment.</returns>
        // POST: Enrollments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [VolunteersCustomAuthorization(UserRolePermission = EnumRole.Admin)]
        public ActionResult DeleteConfirmed(int id)
        {
            var enrollment = enrollmentRepository.GetEnrollmentById(id);
            enrollmentRepository.DeleteEnrollment(enrollment);


            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Volunteer email answer.
        /// </summary>
        /// <param name="contributionId">Contribution id.</param>
        /// <param name="volunteerId">Volunteer id.</param>
        /// <returns>Volunteer email answer.</returns>
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

        /// <summary>
        /// Save volunteer email answer having accept or decline status.
        /// </summary>
        /// <param name="form">Form collention.</param>
        /// <param name="contributionId">Contribution id.</param>
        /// <param name="volunteerId">Volunteer id.</param>
        /// <returns>Save volunteer email answer.</returns>
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
