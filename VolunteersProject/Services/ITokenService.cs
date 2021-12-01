﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VolunteersProject.DTO;

namespace VolunteersProject.Services
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, UserDTO user);
        //string GenerateJSONWebToken(string key, string issuer, UserDTO user);
        bool IsTokenValid(string key, string issuer, string token);
    }
}
