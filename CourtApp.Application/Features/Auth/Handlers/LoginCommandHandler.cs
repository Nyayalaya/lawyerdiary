using AspNetCoreHero.Results;
using CourtApp.Application.Features.Auth.Commands;
using CourtApp.Application.Features.Auth.Dto;
using CourtApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Auth.Handlers
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokenResponse>>
    {
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<TokenResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var tokenRequest = new TokenRequest
            {
                Email = request.Email,
                Password = request.Password
            };

            return await _identityService.GetTokenAsync(tokenRequest, request.IpAddress);
        }
    }
}
