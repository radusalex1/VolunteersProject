using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        public int Age
        {
            get
            {
                return DateTime.Today.Year - BirthDate.Year;
            }
        }
        public ICollection<Enrollment> Enrollments{ get; set; }
        public string FullName
        {
            get
            {
                return $"{this.Name} {this.Surname}";
            }
        }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoinHubDate { get; set; }
      
    }
}
