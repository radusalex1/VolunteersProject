using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VolunteersProject.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class CurrentUser
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        [Required]
        [Display(Name = "Nume", Prompt = "Family Name/Last Name")]
        public string Name { get; set; } = null!;

        /// <summary>
        /// Gets or sets the first/sur name.
        /// </summary>
        [Required]
        [Display(Name = "Prenume", Prompt = "First Name")]
        public string Surname { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Name="Username",Prompt ="Username")]
        public string UserName { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required]
        [Display(Prompt = "example@example.org")]
        public string Email { get; set; } = null!;

        /// <summary>
        /// Get or sets the phone.
        /// </summary>
        [Required]
        public string Phone { get; set; } = null!;

        /// <summary>
        /// 
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime BirthDate { get; set; }

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
        
        /// <summary>
        /// Gets or sets the image profile.
        /// </summary>
        [NotMapped]
        public IFormFile ImageProfile { get; set; }

        /// <summary>
        /// Gets or set the description contribution to hub.
        /// </summary>
        public string DescriptionContributionToHub { get; set; }
    }
}
