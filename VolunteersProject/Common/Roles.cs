using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.Common
{
    public static class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
    }

    public enum EnumRole
    {
        Admin = 1,
        //PowerUser = 1,
        User = 2
    }
}
