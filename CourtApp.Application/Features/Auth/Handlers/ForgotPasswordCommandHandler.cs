using AspNetCoreHero.Results;
using CourtApp.Application.Features.Auth.Commands;
using CourtApp.Application.Features.Auth.Dto;
using CourtApp.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CourtApp.Application.Features.Auth.Handlers
{
    public class ForgotPasswordCommandHandler
    : IRequestHandler<ForgotPasswordCommand, Result<string>>
    {
        private readonly IIdentityService _identityService;

        public ForgotPasswordCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Result<string>> Handle(
            ForgotPasswordCommand request,
            CancellationToken cancellationToken)
        {
            var forgotRequest = new ForgotPasswordRequest
            {
                Email = request.Email
            };

            await _identityService.ForgotPassword(forgotRequest, request.Origin);

            return Result<string>.Success(
                "If the email exists, a reset link has been sent.");
        }
    }
}
