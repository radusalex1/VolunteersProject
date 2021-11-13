using System.Collections.Generic;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IContributionRepository
    {
        Contribution GetContributionById(int id);
        List<Contribution> GetContributions();
    }
}