using VolunteersProject.DTO;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IUserRepository
    {
        UserDTO GetUser(UserModel userMode);
    }
}
