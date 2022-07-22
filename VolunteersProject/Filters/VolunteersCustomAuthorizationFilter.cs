﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using VolunteersProject.Common;

namespace VolunteersProject.Filters
{
    public class VolunteersCustomAuthorization : AuthorizeAttribute, IAuthorizationFilter
    {
        public EnumRole Permissions { get; set; } //Permission string to get from controller

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            //Validate if any permissions are passed when using attribute at controller or action level
            //if (string.IsNullOrEmpty(Permissions))
            //{
            //    //Validation cannot take place without any permissions so returning unauthorized
            //    context.Result = new UnauthorizedResult();
            //    return;
            //}


            //The below line can be used if you are reading permissions from token
            //var permissionsFromToken=context.HttpContext.User.Claims.Where(x=>x.Type=="Permissions").Select(x=>x.Value).ToList()

            //Identity.Name will have windows logged in user id, in case of Windows Authentication
            //Indentity.Name will have user name passed from token, in case of JWT Authenntication and having claim type "ClaimTypes.Name"
            //var userName = context.HttpContext.User.Identity.Name;
            //var assignedPermissionsForUser = MockData.UserPermissions.Where(x => x.Key == userName).Select(x => x.Value).ToList();

            //Multiple permissiosn can be received from controller, delimiter "," is used to get individual values
            //var requiredPermissions = Permissions.Split(","); 

            //foreach (var x in requiredPermissions)
            //{
            //    if (assignedPermissionsForUser.Contains(x))
            //        return; //User Authorized. Wihtout setting any result value and just returning is sufficent for authorizing user
            //}

            //get role fo the current User
            var currentUserRoleId = 2;

            if (currentUserRoleId <= (int)Permissions)
            {
                return; //User Authorized. Wihtout setting any result value and just returning is sufficent for authorizing user
            }

            context.Result = new UnauthorizedResult();

            return;
        }
    }
}