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

        bool VolunteerExists(Volunteer volunteer);

        void DeleteVolunteer(Volunteer volunteer);
    }
}
