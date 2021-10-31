using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly VolunteersContext _context;

        public VolunteerRepository(VolunteersContext context)
        {
            _context = context;
        }

        public List<Volunteer> GetUnselectedVolunteers(int ContributionID)
        {
            return _context.Volunteers
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.contribution)
                .ToList();
        }

        public Volunteer GetVolunteerById(int id)
        {
            return _context.Volunteers.FirstOrDefault(i => i.ID.Equals(id));
        }

        public List<Volunteer> GetVolunteers()
        {            
            return _context.Volunteers.ToList();
        }
        
    }
}
