using System.Collections.Generic;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IVolunteerRepository
    {
        List<Volunteer> GetVolunteers();

        Volunteer GetVolunteerById(int? id);

        Volunteer GetVolunteerWithEnrollmentsById(int? id);

        List<Volunteer> GetAvailableVolunteers(int? ContributionID);

        void AddVolunteer(Volunteer volunteer);

        void UpdateVolunteer(Volunteer volunteer);

        bool VolunteerExists(int id);

        void DeleteVolunteer(Volunteer volunteer);

        //void UpdateVolunteer(Volunteer volunteer);
    }
}
