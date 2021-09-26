using System.Collections.Generic;

namespace VolunteersProject.Models
{
    public class Contribution
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
