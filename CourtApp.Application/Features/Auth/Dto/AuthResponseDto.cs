using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Auth.Dto
{
    public class AuthResponseDto
    {
        public bool Status { get; set; }

        public string Message { get; set; }

        public string Token { get; set; }

        public string UserId { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }

}
