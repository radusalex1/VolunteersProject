using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    /// <summary>
    /// Contribution repository.
    /// </summary>
    public class ContributionRepository : IContributionRepository
    {
        private readonly VolunteersContext _context;
        private readonly ILogger<ContributionRepository> logger;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="logger">Logger.</param>
        /// <param name="context">context.</param>
        public ContributionRepository(ILogger<ContributionRepository> logger, VolunteersContext context)
        {
            _context = context;
            this.logger = logger;
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
            this.logger.LogInformation("GetContributions().");

            try
            {
                if (_context == null)
                {
                    this.logger.LogInformation("_context is null");

                    return null;
                }

                if (_context.Contributions == null)
                {
                    this.logger.LogInformation("_context.Contributions");
                    return null;
                }
            }
            catch(Exception ex)
            {
                this.logger.LogInformation(ex.Message);
            }

            return _context.Contributions.ToList();
        }

        public void AddContribution(Contribution contribution)
        {
            _context.Add(contribution);
            _context.SaveChanges();
        }
        public void UpdateContribution(Contribution contribution)
        {
            _context.Update(contribution);
            _context.SaveChanges();
        }
        public void DeleteContribution(Contribution contribution)
        {
            _context.Remove(contribution);
            _context.SaveChanges();
        }
        public bool ContributionExists(int id)
        {
            return _context.Contributions.Any(e => e.ID == id);
        }
    }
}
