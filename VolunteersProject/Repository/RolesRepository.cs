using System;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly VolunteersContext _context;

        public RolesRepository(VolunteersContext context)
        {
            _context = context;
        }

        public Role GetAdminRight()
        {
            return _context.Roles.FirstOrDefault(r => r.Name == "Admin");
        }

        public Role GetUserRight()
        {
            var test =  _context.Roles.FirstOrDefault(r => r.Name == "User");
            return test;
        }

    }
}
