using System.Collections.Generic;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IVolunteerRepository
    {
        List<Volunteer> GetVolunteers();

        Volunteer GetVolunteerById(int? id);

        /// <summary>
        /// Get volunteer with related enrollments.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns>Return colunteer with related enrollments.</returns>
        Volunteer GetVolunteerWithEnrollmentsById(int? id);

        List<Volunteer> GetAvailableVolunteers(int ContributionID);

        void AddVolunteer(Volunteer volunteer);

        void UpdateVolunteer(Volunteer volunteer);

        bool VolunteerExists(int id);

        /// <summary>
        /// Search by phone, email if volunteer exist.
        /// </summary>
        /// <param name="volunteer"></param>
        /// <returns>True if exist, otherwise false.</returns>
        bool CheckVolunteerExistByPhoneOrEmail(Volunteer volunteer);

        void DeleteVolunteer(Volunteer volunteer);
    }
}
