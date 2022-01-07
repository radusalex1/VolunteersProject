using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.Models
{
    public class CurrentUser
    {
        public Volunteer Volunteer { get; set; }
        public User User { get; set; }
    }
}
