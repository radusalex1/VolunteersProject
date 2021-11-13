using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public class ContributionRepository : IContributionRepository
    {
        private readonly VolunteersContext _context;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="context"></param>
        public ContributionRepository(VolunteersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get volunteer by id.
        /// </summary>
        /// <param name="id">Volunteer id.</param>
        /// <returns></returns>
        public Contribution GetContributionById(int id)
        {
            return _context.Contributions.FirstOrDefault(i => i.ID.Equals(id));
        }

        /// <summary>
        /// Get all volunteers.
        /// </summary>
        /// <returns>List of all volunteers.</returns>
        public List<Contribution> GetContributions()
        {
            return _context.Contributions.ToList();
        }
    }
}
