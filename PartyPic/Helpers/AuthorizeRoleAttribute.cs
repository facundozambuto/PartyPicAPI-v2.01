
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PartyPic.Models.Users;
using System;
using System.Linq;

namespace PartyPic.Helpers
{
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedRoles;

        public AuthorizeRoleAttribute(params int[] roles)
        {
            _allowedRoles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];

            if (user == null)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!_allowedRoles.Contains(user.RoleId))
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }

}
