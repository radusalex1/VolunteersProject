using Microsoft.EntityFrameworkCore;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.DTO;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    /// <summary>
    /// Repository for Users
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly VolunteersContext _context;
        private readonly IVolunteerRepository _volunteerRepository;
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(VolunteersContext context,IVolunteerRepository volunteerRepository)
        {
            _context = context;
            _volunteerRepository = volunteerRepository;
        }
      
        /// <summary>
        /// get user
        /// </summary>
        /// <param name="userModel"></param>
        /// <returns></returns>
        public User GetUser(LoginModel userModel)
        {
            return _context.Users
                .Include(r => r.Role)
                .Where(x => x.UserName.ToLower() == userModel.UserName.ToLower()
                && x.Password == userModel.Password).FirstOrDefault();
        }

        /// <summary>
        /// add user to database
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public int AddUser(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
            return user.Id; 
        }

        /// <summary>
        /// method to check username unique
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public bool AlreadyUseUsername(string username)
        {
            return _context.Users.Any(e => e.UserName == username);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currentUser"></param>
        /// <returns></returns>
        public bool AlreadyUserUsername_OnEditPersonalInfo(CurrentUser currentUser)
        {
            var result = _context.Users.FirstOrDefault(u => u.UserName == currentUser.UserName);

            var currentVolunteer = _volunteerRepository.GetVolunteerById(currentUser.Id);

            if (result == null)
            {
                return false;
            }


            if (result.Id == currentVolunteer.User.Id)
            {
                return false;
            }
                
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="NewPassword"></param>
        public void ChangePasswordBasedOnUserId(int id, string NewPassword)
        {
            var user = _context.Users
                 .FirstOrDefault(u => u.Id == id);
            user.Password = NewPassword;
            _context.Update(user);
            _context.SaveChanges();
                
        }

        /// <summary>
        /// Returns user by its id;
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetUserById(int id)
        {
            return _context.Users
                .FirstOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Deletes this user;
        /// </summary>
        /// <param name="user"></param>
        public void DeteleUser(User user)
        {
            _context.Users.Remove(user);
            _context.SaveChanges();
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(User user)
        {
            var user_result = _context.Users
                 .FirstOrDefault(u => u.Id == user.Id);

            user_result.UserName = user.UserName;

            _context.Update(user_result);
            _context.SaveChanges();

        }
    }
}
