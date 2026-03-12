using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CourtApp.Api.Models;
using AspNetCoreHero.Results;

namespace CourtApp.Api.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private UserContextInfo _currentUser;

        protected BaseController(
            IMediator mediator,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _httpContextAccessor = httpContextAccessor;
        }

        protected IMediator Mediator => _mediator;

        protected CancellationToken RequestAborted =>
            _httpContextAccessor?.HttpContext?.RequestAborted ?? CancellationToken.None;

        /// <summary>
        /// Current User Context
        /// </summary>
        protected UserContextInfo CurrentUser =>
            _currentUser ??= ExtractUserContext();

        protected string UserId => CurrentUser?.UserId;

        protected string UserEmail => CurrentUser?.Email;

        protected string UserName => CurrentUser?.UserName;

        protected List<string> Roles => CurrentUser?.Roles ?? new();

        /// <summary>
        /// Extract User Information from Claims
        /// </summary>
        private UserContextInfo ExtractUserContext()
        {
            var httpContext = _httpContextAccessor?.HttpContext;

            if (httpContext == null)
                return new UserContextInfo();

            var user = httpContext.User;

            var context = new UserContextInfo
            {
                IsAuthenticated = user?.Identity?.IsAuthenticated ?? false,
                IpAddress = GetClientIpAddress(),
                CorrelationId = httpContext.TraceIdentifier
            };

            if (!context.IsAuthenticated)
                return context;

            context.UserId =
                user.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                user.FindFirst("uid")?.Value;

            context.UserName =
                user.FindFirst(ClaimTypes.Name)?.Value ??
                user.FindFirst("username")?.Value;

            context.Email =
                user.FindFirst(ClaimTypes.Email)?.Value;

            context.FirstName =
                user.FindFirst("first_name")?.Value ??
                user.FindFirst(ClaimTypes.GivenName)?.Value;

            context.LastName =
                user.FindFirst("last_name")?.Value ??
                user.FindFirst(ClaimTypes.Surname)?.Value;

            context.FullName =
                user.FindFirst("full_name")?.Value ??
                $"{context.FirstName} {context.LastName}".Trim();

            context.Mobile =
                user.FindFirst("mobile")?.Value ??
                user.FindFirst(ClaimTypes.MobilePhone)?.Value;

            context.Roles = user
                .FindAll("roles")
                .Select(x => x.Value)
                .ToList();

            context.Claims = user.Claims.ToList();

            return context;
        }

        /// <summary>
        /// Role Checking
        /// </summary>
        protected bool HasRole(string role)
        {
            return Roles.Contains(role);
        }

        protected bool HasAnyRole(params string[] roles)
        {
            return Roles.Intersect(roles).Any();
        }

        /// <summary>
        /// Detect Client IP (Proxy safe)
        /// </summary>
        protected string GetClientIpAddress()
        {
            var context = _httpContextAccessor?.HttpContext;

            if (context == null)
                return "Unknown";

            if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwarded))
                return forwarded.FirstOrDefault()?.Split(',').FirstOrDefault();

            if (context.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
                return realIp;

            return context.Connection.RemoteIpAddress?.ToString();
        }

        // -------------------------------
        // STANDARD API RESPONSES
        // -------------------------------

        protected IActionResult Success<T>(T data, string message = "Success")
        {
            return Ok(ApiResponse<T>.Success(data, message));
        }

        protected IActionResult CreatedResponse<T>(T data, string message = "Created")
        {
            return StatusCode(201, ApiResponse<T>.Success(data, message, 201));
        }

        protected IActionResult Failure(string message)
        {
            return BadRequest(ApiResponse<object>.Failure(message));
        }

        protected IActionResult UnauthorizedResponse()
        {
            return Unauthorized(ApiResponse<object>.Failure("Unauthorized"));
        }

        protected IActionResult ForbiddenResponse()
        {
            return StatusCode(403, ApiResponse<object>.Failure("Forbidden"));
        }

        protected IActionResult NotFoundResponse(string message = "Not Found")
        {
            return NotFound(ApiResponse<object>.Failure(message));
        }

        protected IActionResult ValidationError(List<string> errors)
        {
            return UnprocessableEntity(ApiResponse<object>.Failure("Validation Failed", 422, errors));
        }

        protected IActionResult ServerError(string message = "Internal Server Error")
        {
            return StatusCode(500, ApiResponse<object>.Error(message));
        }

        /// <summary>
        /// Convert AspNetCoreHero Result to ApiResponse
        /// </summary>
        protected IActionResult FromResult<T>(Result<T> result)
        {
            if (result.Succeeded)
                return Success(result.Data, result.Message);

            return Failure(result.Message);
        }
    }
}
