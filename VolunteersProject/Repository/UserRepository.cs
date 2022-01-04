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

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        public UserRepository(VolunteersContext context)
        {
            _context = context;
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
    }
}
