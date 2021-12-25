using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VolunteersProject.Models
{
    /// <summary>
    /// Volunteer model.
    /// </summary>
    public class Volunteer
    {
        /// <summary>
        /// Gets or sets the volunteer id.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Display(Name="Nume",Prompt ="Family Name/Last Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first/sur name.
        /// </summary>
        [Display(Name ="Prenume",Prompt ="First Name")]
        public string Surname { get; set; }

        public string City { get; set; }

        [Display(Prompt ="example@example.org")]
        public string Email { get; set; }

        /// <summary>
        /// Get or sets the phone.
        /// </summary>
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }
        
        /// <summary>
        /// Gets the name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        [Display(Name="Volunteer")]
        public string FullName
        {
            get
            {
                return $"{this.Name} {this.Surname}";
            }
        }

        /// <summary>
        /// Get or sets the join date to the hub.
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime JoinHubDate { get; set; }

        /// <summary>
        /// Gets or sets the instagram profile.
        /// </summary>
        [Display(Prompt = "@example")]
        public string InstagramProfile { get; set; }

        /// <summary>
        /// Gets or sets the Facebook profile.
        /// </summary>
        [Display(Name="Facebook profile link",Prompt ="Facebook profile link")]
        public string FaceBookProfile { get; set; }
        
        /// <summary>
        /// Gets or set the description contribution to hub.
        /// </summary>
        public string DescriptionContributionToHub { get; set; }

        /// <summary>
        /// Gets or sets the image profile.
        /// </summary>
        [NotMapped]
        public IFormFile ImageProfile { get; set; }

        /// <summary>
        /// Gets or sets the image profile byte array.
        /// </summary>
        public Byte[] ImageProfileByteArray { get; set; }
    }
}
