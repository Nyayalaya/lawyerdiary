using AspNetCoreHero.Results;
using CourtApp.Application.Features.Auth.Dto;
using CourtApp.Application.Features.Auth.Handlers;
using CourtApp.Application.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Auth.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;

        public RegisterUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<string>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var registerRequest = new RegisterRequest
            {
                Email = request.Email,
                UserName = request.UserName,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName
            };

            return await _identityService.RegisterAsync(registerRequest, request.Origin);
        }
    }
}
