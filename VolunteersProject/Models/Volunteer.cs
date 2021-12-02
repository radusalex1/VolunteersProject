using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VolunteersProject.Models
{
    /// <summary>
    /// Volunteer model.
    /// </summary>
    public class Volunteer
    {
        public int ID { get; set; }

        [Display(Name="Nume",Prompt ="Family Name/Last Name")]
        public string Name { get; set; }
        
        [Display(Name ="Prenume",Prompt ="First Name")]
        public string Surname { get; set; }

        public string City { get; set; }

        [Display(Prompt ="example@example.org")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        
        public int Age
        {
            get
            {
                DateTime now = DateTime.Today;
                int age = now.Year - BirthDate.Year;
                if (now < BirthDate.AddYears(age))
                    age--;
                return age;
            }
        }
        
        public ICollection<Enrollment> Enrollments{ get; set; }
        [Display(Name="Volunteer")]
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

        [Display(Prompt = "@example")]
        public string InstagramProfile { get; set; }

        [Display(Name="Facebook profile link",Prompt ="Facebook profile link")]
        public string FaceBookProfile { get; set; }
        
        public string DescriptionContributionToHub { get; set; }
      
        public bool IsSelected { get; set; }
    }
}
