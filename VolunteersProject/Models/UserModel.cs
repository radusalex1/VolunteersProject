using System.ComponentModel.DataAnnotations;

namespace VolunteersProject.Models
{
    //todo Radu : move this in DTO folder and rename it as LoginDTO
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
