
namespace VolunteersProject.DTO
{
    //todo Radu - this DTO will not be used when user is read from DB. So remove UserDTO.cs and use only User.cs
    public class UserDTO
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}
