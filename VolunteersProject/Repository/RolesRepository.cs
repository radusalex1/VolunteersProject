using System;
using System.Linq;
using VolunteersProject.Data;
using VolunteersProject.Models;

namespace VolunteersProject.Repository
{
    public class RolesRepository : IRolesRepository
    {
        private readonly VolunteersContext _context;

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="context"></param>
        public RolesRepository(VolunteersContext context)
        {
            _context = context;
        }

        /// <summary>
        /// get admin right
        /// </summary>
        /// <returns></returns>
        public Role GetAdminRight()
        {
            return _context.Roles.FirstOrDefault(r => r.Name == "Admin");
        }

        /// <summary>
        /// get user right
        /// </summary>
        /// <returns></returns>
        public Role GetUserRight()
        {
            var test =  _context.Roles.FirstOrDefault(r => r.Name == "User");
            return test;
        }

    }
}
