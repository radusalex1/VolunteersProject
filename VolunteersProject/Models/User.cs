using Newtonsoft.Json;
using System.Collections.Generic;

namespace VolunteersProject.Models
{
    /// <summary>
    /// Used to manipulate logged user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public int Id { get; set; }
                
        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Gets or sets the user role.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Gets or sets assigned volunteers.
        /// </summary>
        public ICollection<Volunteer> Volunteers { get; set; }

        /// <summary>
        /// Gets or sets password.
        /// </summary>
        [JsonIgnore]
        public string Password { get; set; }
    }
}
