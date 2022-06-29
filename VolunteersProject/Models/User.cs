using Newtonsoft.Json;
using System.Collections.Generic;

namespace VolunteersProject.Models
{
    /// <summary>
    /// used to get logged user from dbo.
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }

        public ICollection<Volunteer> Volunteers { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}
