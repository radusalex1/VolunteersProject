using System.Collections.Generic;

namespace VolunteersProject.Models
{
    public class SelectedVolunters
    {
        public int ContributionId { get; set; }

        public List<Volunteer> Volunteers { get; set; }
    }
}
