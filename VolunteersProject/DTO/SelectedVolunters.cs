using System.Collections.Generic;

namespace VolunteersProject.DTO
{
    /// <summary>
    /// DTO that contains available volunteers for a contribution id.
    /// </summary>
    public class AvailableVolunters
    {
        /// <summary>
        /// Gets or sets the contribution id.
        /// </summary>
        public int ContributionId { get; set; }

        /// <summary>
        /// Gets or sets the volunteersDTO list.
        /// </summary>
        public List<VolunteerDTO> VolunteersDTO { get; set; }
    }
}
