using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.DTO
{
    
    public class EnterEmailForPasswordRecoveryDTO
    {

        public int Id { get; set; }

        /// <summary>
        /// Email for 
        /// </summary>
        [Required]
        [Display(Prompt = "example@example.org")]
        public string Email { get; set; }
    }
}
