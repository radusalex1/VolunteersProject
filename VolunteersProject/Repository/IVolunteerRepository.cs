using System.Collections.Generic;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IVolunteerRepository
    {
        List<Volunteer> GetVolunteers();

        Volunteer GetVolunteerById(int id);

        List<Volunteer> GetAvailableVolunteers(int ContributionID);
    }
}
