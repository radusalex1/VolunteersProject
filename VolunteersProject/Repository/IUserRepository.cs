using VolunteersProject.DTO;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IUserRepository
    {
        /// <summary>
        /// Get User based on login credentials
        /// </summary>
        /// <param name="userMode"></param>
        /// <returns></returns>
        User GetUser(LoginModel userMode);
        
        /// <summary>
        /// Add User
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        int AddUser(User user);

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        bool AlreadyUseUsername(string username);

        /// <summary>
        /// Change password based on user id
        /// </summary>
        /// <param name="NewPassword"></param>
        void ChangePasswordBasedOnUserId(int id,string NewPassword);
    }
}
