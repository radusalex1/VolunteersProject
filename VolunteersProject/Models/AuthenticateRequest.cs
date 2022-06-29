using System.ComponentModel.DataAnnotations;

namespace VolunteersProject.Models
{
    /// <summary>
    /// Authentication model.
    /// </summary>
    public class AuthenticateRequest
    {
        /// <summary>
        /// User name.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Password.
        /// </summary>
        [Required]
        public string Password { get; set; }
    }
}
