using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IVolunteerRepository
    {
        List<Volunteer> GetVolunteers();

        Volunteer GetVolunteerById(int id);
    }
}
