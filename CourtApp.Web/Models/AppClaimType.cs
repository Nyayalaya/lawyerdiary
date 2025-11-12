using System.Security.Claims;

namespace CourtApp.Web.Models
{
    public static class AppClaimType
    {
        // Standard
        public const string Name = ClaimTypes.Name;
        public const string NameIdentifier = ClaimTypes.NameIdentifier;
        public const string Email = ClaimTypes.Email;
        public const string GivenName = ClaimTypes.GivenName;
        public const string Surname = ClaimTypes.Surname;
        public const string StreetAddress = ClaimTypes.StreetAddress;
        public const string MobilePhone = ClaimTypes.MobilePhone;
        public const string Role = ClaimTypes.Role;

        // Custom
        public const string LinkedIds = "LinkedIds";
        public const string OfficeAddress = "OfficeAddress";
        public const string EnrollmentNo = "EnrollmentNo";
    }
}
