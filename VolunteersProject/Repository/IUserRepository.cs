using VolunteersProject.DTO;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IUserRepository
    {
        User GetUser(UserModel userMode);
    }
}
