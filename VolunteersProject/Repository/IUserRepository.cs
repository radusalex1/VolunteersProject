using VolunteersProject.DTO;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IUserRepository
    {
        User GetUser(LoginModel userMode);
        void AddUser(User user);
    }
}
