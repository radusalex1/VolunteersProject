﻿using VolunteersProject.DTO;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public interface IUserRepository
    {
        User GetUser(LoginModel userMode);
        int AddUser(User user);
        bool AlreadyUseUsername(string username);
    }
}
