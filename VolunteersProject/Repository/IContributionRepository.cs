using System.Collections.Generic;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IContributionRepository
    {
        Contribution GetContributionById(int id);
        List<Contribution> GetContributions();
        void AddContribution(Contribution contribution);
        void UpdateContribution(Contribution contribution);
        void DeleteContribution(Contribution contribution);
        bool ContributionExists(int id);
    }
}