using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    /// <summary>
    /// Repository for volunteers.
    /// </summary>
    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly VolunteersContext _context;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="context"></param>
        public VolunteerRepository(VolunteersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all not assigned volunteers to the current contribution id.
        /// </summary>
        /// <param name="contributionId">Contribution id.</param>
        /// <returns>List of not assigned volunteers.</returns>
        public List<Volunteer> GetAvailableVolunteers(int contributionId)
        {
            var volunteers = _context.Volunteers
             .Include(e => e.Enrollments)
             .ThenInclude(c => c.contribution)
             .ToList();            

            var volunteersAssigned = volunteers.Where(v => v.Enrollments.Any(c => c.contribution.ID == contributionId));

            var volunteersAvailable = volunteers.Where(v => v.Enrollments.Any(c => c.contribution.ID != contributionId) && !volunteersAssigned.Contains(v)).ToList();

            var volunteersWithNoAnyAssignments = _context.Volunteers.Where(v => v.Enrollments.Count == 0).ToList();

            return volunteersAvailable.Union(volunteersWithNoAnyAssignments).ToList();
        }

        /// <summary>
        /// Get volunteer by id.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns></returns>
        public Volunteer GetVolunteerById(int id)
        {
            return _context.Volunteers.FirstOrDefault(i => i.ID.Equals(id));
        }

        /// <summary>
        /// Get all volunteers.
        /// </summary>
        /// <returns>List of all volunteers.</returns>
        public List<Volunteer> GetVolunteers()
        {
            return _context.Volunteers.ToList();
        }
    }
}
