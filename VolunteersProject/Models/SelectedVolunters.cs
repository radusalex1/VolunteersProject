using System.Collections.Generic;

namespace VolunteersProject.Models
{
    //todo cia - this is also ugly
    public class SelectedVolunters
    {
        public int ContributionId { get; set; }

        public List<Volunteer> Volunteers { get; set; }
    }
}
