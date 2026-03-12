using CourtApp.Application.Features.Auth.Commands;
using CourtApp.Application.Features.Auth.Handlers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CourtApp.Api.Controllers
{
    public class AuthController : BaseController
    {
        public AuthController(
            IMediator mediator,
            IHttpContextAccessor accessor)
            : base(mediator, accessor)
        {
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterCommand command)
        {
            command.Origin = $"{Request.Scheme}://{Request.Host}";
            var result = await Mediator.Send(command, RequestAborted);
            return FromResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginCommand command)
        {
            command.IpAddress = GetClientIpAddress();
            var result = await Mediator.Send(command, RequestAborted);
            return FromResult(result);
        }

        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailCommand command)
        {
            var result = await Mediator.Send(command, RequestAborted);
            return FromResult(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordCommand command)
        {
            command.Origin = $"{Request.Scheme}://{Request.Host}";
            var result = await Mediator.Send(command, RequestAborted);
            return FromResult(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordCommand command)
        {
            var result = await Mediator.Send(command, RequestAborted);
            return FromResult(result);
        }
    }
}
