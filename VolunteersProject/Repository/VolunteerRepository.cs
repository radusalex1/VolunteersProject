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

        public Volunteer GetVolunteerById(int id)
        {
            return _context.Volunteers.FirstOrDefault(i => i.ID.Equals(id));
        }

        public List<Volunteer> GetVolunteers()
        {
            //return _context.Volunteers.ToListAsync();
            return _context.Volunteers.ToList();
        }
    }
}
