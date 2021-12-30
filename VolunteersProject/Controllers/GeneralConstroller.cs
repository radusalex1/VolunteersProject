using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.Repository;

namespace VolunteersProject.Controllers
{

    public class GeneralConstroller : Controller
    {
        public readonly ILogger<GeneralConstroller> Logger;
        public IConfiguration configuration;

        public GeneralConstroller(ILogger<GeneralConstroller> logger, IConfiguration configuration)
        {
            this.Logger = logger;
            this.configuration = configuration;
        }

    }
}
