using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.Models
{
    public class Volunteer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string City { get; set; }
        public ICollection<Enrollment> Enrollments{ get; set; }
        public string FullName
        {
            get
            {
                return $"{this.Name} {this.Surname}";
            }
        }
    }
}
