using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.Models
{
    public class Roles
    {
        public int Id { get; set; }

        /// <summary>
        /// administrator,simple user etc;
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// the more power the more rights the user has
        /// </summary>
        public int Power { get; set; }
    }
}
