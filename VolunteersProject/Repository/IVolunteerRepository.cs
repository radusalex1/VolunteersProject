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
        IQueryable<Volunteer> GetAvailableVolunteers(int contributionId);

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
    }
}
