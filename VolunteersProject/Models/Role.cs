using System.Collections.Generic;

namespace VolunteersProject.Models
{
    public class Role
    {
        public int Id { get; set; }

        /// <summary>
        /// administrator,simple user etc;
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// the more power the more rights the user has
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// administrator,simple user etc;
        /// </summary>
        public string Description { get; set; }

        public ICollection<User> Users { get; set; }
    }
}
