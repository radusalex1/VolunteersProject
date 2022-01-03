using System;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly VolunteersContext _context;

        public Role GetAdminRight()
        {
            return _context.Roles.FirstOrDefault(r => r.Description == "Admin");

        }

        public Role GetUserRight()
        {
            return _context.Roles.FirstOrDefault(r => r.Description == "User");
        }

    }
}
