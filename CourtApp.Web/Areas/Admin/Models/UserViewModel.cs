using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace CourtApp.Web.Areas.Admin.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; } = true;
        public string EnrollmentNo { get; set; }
        public string Mobile { get; set; }
        public string Website { get; set; }
        public string Telephone { get; set; }
        public string Address { get; set; }
        public string ProfileImgPath { get; set; }
        public byte[] ProfilePicture { get; set; }
        public bool EmailConfirmed { get; set; }
        public string Id { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? DateOfJoining { get; set; }
        public SelectList Genders { get; set; }
        public string Role { get; set; }
        public SelectList Roles { get; set; }
        public LaywerViewModel LawyerInfo { get; set; }
        public OperatorViewModel OperInfo { get; set; }
    }
}