using System;

namespace VolunteersProject.DTO
{
    /// <summary>
    /// DTO for volunteer.
    /// </summary>
    public class VolunteerDTO
    {
        /// <summary>
        /// Gets or sets the volunteer id.
        /// </summary>
        public int ID { get; set; }

        public string FullName { get; set; }

        public DateTime JoinHubDate { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public bool IsSelected { get; set; }
    }
}
