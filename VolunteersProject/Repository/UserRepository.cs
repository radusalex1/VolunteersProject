﻿using System.Collections.Generic;
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

        public UserRepository(VolunteersContext context)
        {
            _context = context;
        }
      
        

        public User GetUser(UserModel userModel)
        {
            return _context.Users.Where(x => x.UserName.ToLower() == userModel.UserName.ToLower()
                && x.Password == userModel.Password).FirstOrDefault();
        }
    }
}
