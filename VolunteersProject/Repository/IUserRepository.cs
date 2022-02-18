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
        /// 
        /// </summary>
        /// <returns></returns>
        bool AlreadyUserUsername_OnEditPersonalInfo(CurrentUser currentUser);

        /// <summary>
        /// Change password based on user id
        /// </summary>
        /// <param name="NewPassword"></param>
        void ChangePasswordBasedOnUserId(int id,string NewPassword);

        /// <summary>
        /// Get user by id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        User GetUserById(int id);

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="user"></param>
        void DeteleUser(User user);

        /// <summary>
        /// Update a User
        /// </summary>
        /// <param name="user"></param>
        void UpdateUser(User user);
    }
}
