using AspNetCoreHero.Results;
using CourtApp.Application.Features.Auth.Dto;
using MediatR;

namespace CourtApp.Application.Features.Auth.Handlers
{
    public class RegisterCommand : IRequest<Result<string>>
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Origin { get; set; }
    }
}
