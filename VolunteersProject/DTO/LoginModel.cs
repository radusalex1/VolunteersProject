using System.ComponentModel.DataAnnotations;

namespace VolunteersProject.DTO
{
    /// <summary>
    /// login model
    /// </summary>
    public class LoginModel
    {
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
