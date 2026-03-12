using System.ComponentModel.DataAnnotations;

namespace CourtApp.Application.Features.Auth.Dto
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}