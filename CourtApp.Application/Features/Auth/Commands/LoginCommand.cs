using AspNetCoreHero.Results;
using CourtApp.Application.Features.Auth.Dto;
using MediatR;

namespace CourtApp.Application.Features.Auth.Commands
{
    public class LoginCommand : IRequest<Result<TokenResponse>>
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public string IpAddress { get; set; }
    }
}
