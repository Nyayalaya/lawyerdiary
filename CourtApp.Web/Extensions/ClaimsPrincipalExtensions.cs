using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
namespace CourtApp.Web.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static List<string> GetUserLinkedIds(this ClaimsPrincipal user)
        {
            var raw = user?.FindFirst("LinkedIds")?.Value;
            return raw?.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
        }

        //public static List<string> GetRoles(this ClaimsPrincipal user)
        //{
        //    var raw = user?.FindFirst("Roles")?.Value;
        //    return raw?.Split(",", StringSplitOptions.RemoveEmptyEntries).ToList() ?? new List<string>();
        //}
    }
}
