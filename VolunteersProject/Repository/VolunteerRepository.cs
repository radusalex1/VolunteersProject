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

        public List<Volunteer> GetAvailableVolunteers(int ContributionID)
        {
            if(ContributionID==null)
            {
                return new List<Volunteer>();
            }
            var volunteers = _context.Volunteers
                .Include(e => e.Enrollments)
                .ThenInclude(c => c.contribution)
                .ToList();

            var volunteersFiltred = volunteers.Where(v => v.Enrollments.Any(c => c.contribution.ID!= ContributionID)).ToList();

            return volunteersFiltred;
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
