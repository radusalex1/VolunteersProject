﻿using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    /// <summary>
    /// Interface for volunteers repository.
    /// </summary>
    public interface IVolunteerRepository
    {
        /// <summary>
        /// Return the user id of volunteer based on email
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        int ReturnUserIdBasedOnEmail(string Email);

        /// <summary>
        /// Gets the contributions where the volunteer given as parameter participated at.
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        List<Contribution> GetContributionsByVolunteer(Volunteer volunteer);

        /// <summary>
        /// Get all volunteers.
        /// </summary>
        /// <returns>List of all volunteers.</returns>
        //List<Volunteer> GetVolunteers();
        IQueryable<Volunteer> GetVolunteers();

        /// <summary>
        /// Get volunteer by id.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Volunteer.</returns>
        Volunteer GetVolunteerById(int? id);

        /// <summary>
        /// Get volunteer with related enrollments.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Return colunteer with related enrollments.</returns>
        Volunteer GetVolunteerWithEnrollmentsById(int? id);

        /// <summary>
        /// Get all not assigned volunteers to the current contribution id.
        /// </summary>
        /// <param name="contributionId">Contribution id.</param>
        /// <returns>List of not assigned volunteers.</returns>
        //List<Volunteer> GetAvailableVolunteers(int ContributionID);
        List<Volunteer> GetAvailableVolunteers(int contributionId);

        /// <summary>
        /// Add volunteer.
        /// </summary>
        /// <param name="volunteer"></param>
        void AddVolunteer(Volunteer volunteer);

        /// <summary>
        /// Update volunteer.
        /// </summary>
        /// <param name="volunteer">Volunteer.</param>
        void UpdateVolunteer(Volunteer volunteer);

        /// <summary>
        /// Check by id if volunteer exist.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True if exist, otherwise false.</returns>
        bool VolunteerExists(int id);

        /// <summary>
        /// Search by phone, email if volunteer exist.
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns>True if exist, otherwise false.</returns>
        bool CheckVolunteerExistByPhoneOrEmail(Volunteer volunteer);

        /// <summary>
        /// Delete volunteer.
        /// </summary>
        /// <param name="volunteer">Volunteer.</param>
        void DeleteVolunteer(Volunteer volunteer);

        /// <summary>
        /// Returns volunteers total points based on events.
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns></returns>
        int GetVolunteerTotalPoints(Volunteer volunteer);

        /// <summary>
        /// Return the volunteer based on its userId
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Volunteer GetVolunteerByUserId(int id);


        /// <summary>
        /// Return true is email Exists, false otherwise
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        bool EmailExists(string Email);
    }
}
