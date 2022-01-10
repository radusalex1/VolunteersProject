using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VolunteersProject.Models
{

    public class CurrentUser:User
    {
        public int LoggedUserId { get; set; }
    }

}
