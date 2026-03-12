using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace CourtApp.Api.Models
{
    /// <summary>
    /// User context information extracted from JWT token
    /// </summary>
    public class UserContextInfo
    {
        /// <summary>
        /// Unique user identifier
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Username/Login name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// User email address
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name of the user
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name of the user
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// List of roles assigned to the user
        /// </summary>
        public List<string> Roles { get; set; }

        /// <summary>
        /// List of claims associated with the user
        /// </summary>
        public List<Claim> Claims { get; set; }

        /// <summary>
        /// Client IP address from where request originated
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        /// Is user authenticated
        /// </summary>
        public bool IsAuthenticated { get; set; }

        /// <summary>
        /// Mobile number of the user
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// Gender of the user
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Date of birth of the user
        /// </summary>
        public DateTime? DateOfBirth { get; set; }

        /// <summary>
        /// Profile picture URL or path
        /// </summary>
        public string ProfilePictureUrl { get; set; }

        /// <summary>
        /// Check if user has a specific role
        /// </summary>
        public bool HasRole(string role)
        {
            return Roles != null && Roles.Contains(role);
        }

        public string CorrelationId { get; set; }


        /// <summary>
        /// Check if user has any of the specified roles
        /// </summary>
        public bool HasAnyRole(params string[] roles)
        {
            if (Roles == null) return false;
            foreach (var role in roles)
            {
                if (Roles.Contains(role))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if user has all of the specified roles
        /// </summary>
        public bool HasAllRoles(params string[] roles)
        {
            if (Roles == null) return false;
            foreach (var role in roles)
            {
                if (!Roles.Contains(role))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Get claim value by type
        /// </summary>
        public string GetClaimValue(string claimType)
        {
            if (Claims == null) return null;
            return Claims.Find(c => c.Type == claimType)?.Value;
        }
    }
}
