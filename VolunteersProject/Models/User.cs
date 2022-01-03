using Newtonsoft.Json;

namespace VolunteersProject.Models
{
    /// <summary>
    /// used to get logged user from dbo.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Role { get; set; }
        public int RoleId { get; set; }
        [JsonIgnore]
        public string Password { get; set; }
    }
}
