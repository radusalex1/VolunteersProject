using System;
using System.ComponentModel.DataAnnotations;

namespace VolunteersProject.Models
{
    public class RegisterFormModel
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required]
        [Display(Name = "Nume", Prompt = "Family Name/Last Name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the first/sur name.
        /// </summary>
        [Required]
        [Display(Name = "Prenume", Prompt = "First Name")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

        public string City { get; set; }

        [Required]
        [Display(Prompt = "example@example.org")]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate {
            ///not working here
            ///display by default todays date/
            ///validations not working... now. investigate here
            get
            {
                return BirthDate;
            }
            set 
            {
                BirthDate = DateTime.Now;
            } 

        } 

        /// <summary>
        /// Get or sets the join date to the hub.
        /// </summary>
        [Required]
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
        [Display(Name = "Facebook profile link", Prompt = "Facebook profile link")]
        public string FaceBookProfile { get; set; }

    }
}
