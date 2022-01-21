using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.DTO
{
    public class NewPasswordDTO
    {
        public int Id { get; set; }

        public int Email { get; set; }

        public int UserId { get; set; }

        [Required]
        [Display(Prompt ="New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Prompt = "Confirm New Password")]
        public string ConfirmNewPassword { get; set; }

    }
}
