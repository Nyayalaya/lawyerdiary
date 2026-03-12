# CourtApp API - Authentication Implementation Guide

## Overview

This document provides a complete implementation guide for the authentication API in the CourtApp project using .NET 9, Razor Pages, and JWT-based security.

## What's Been Created

### 1. **ApiResponse.cs** - Standard Response Model
- Generic response wrapper `ApiResponse<T>` for typed responses
- Non-generic `ApiResponse` for simple messages
- Factory methods for creating success, failure, and error responses
- Includes status, message, data, statusCode, timestamp, and errors fields

### 2. **UserContextInfo.cs** - User Context Model
- Contains all user information extracted from JWT claims
- Properties: UserId, UserName, Email, FirstName, LastName, FullName, Roles, Claims, etc.
- Helper methods for role checking: `HasRole()`, `HasAnyRole()`, `HasAllRoles()`
- Method to retrieve specific claim value: `GetClaimValue()`

### 3. **BaseController.cs** - Base Controller
- Abstract base class for all API controllers
- Provides user context extraction from JWT tokens
- Helper methods for accessing user information:
  - `GetUserContext()` - Complete user info
  - `GetUserId()` - Current user ID
  - `GetUserName()` - Current username
  - `GetUserEmail()` - Current email
  - `UserHasRole()`, `UserHasAnyRole()`, `UserHasAllRoles()` - Role checking
  - `GetClientIpAddress()` - Extract client IP

- Response helper methods:
  - `SuccessResponse()` - Return success with data
  - `FailureResponse()` - Return failure response
  - `ErrorResponse()` - Return error response
  - `UnauthorizedResponse()`, `ForbiddenResponse()`, `NotFoundResponse()`
  - `ValidationErrorResponse()` - Return validation errors

### 4. **AuthController.cs** - Authentication Endpoints
Complete authentication API with 11 endpoints:
1. **POST /api/auth/login** - User login
2. **POST /api/auth/register** - User registration
3. **POST /api/auth/refresh-token** - Refresh JWT token
4. **POST /api/auth/confirm-email** - Confirm email
5. **POST /api/auth/forgot-password** - Request password reset
6. **POST /api/auth/reset-password** - Reset password
7. **GET /api/auth/profile** - Get complete user profile
8. **GET /api/auth/me** - Get simplified user info
9. **GET /api/auth/roles** - Get user roles
10. **GET /api/auth/claims** - Get user claims
11. **POST /api/auth/verify-token** - Verify token validity

### 5. **ExampleController.cs** - Example Implementation
Demonstrates how to use BaseController features:
- Extracting user context
- Checking roles and permissions
- Returning standardized responses
- Implementing role-based access control

### 6. **Program.cs** - Updated Configuration
- JWT Authentication setup
- CORS policy configuration
- API versioning
- Swagger/OpenAPI integration
- Global exception handling
- Dependency injection configuration

### 7. **API_AUTHENTICATION_DOCUMENTATION.md** - Complete API Documentation
- Standard response format
- All endpoint details with examples
- UserContextInfo and BaseController usage
- Security best practices
- Configuration instructions

## Architecture

```
CourtApp.Api
├── Controllers
│   ├── BaseController.cs (abstract base for all controllers)
│   ├── AuthController.cs (authentication endpoints)
│   └── ExampleController.cs (usage examples)
├── Models
│   ├── ApiResponse.cs (standard response wrapper)
│   └── UserContextInfo.cs (user context info)
├── Program.cs (configuration)
└── API_AUTHENTICATION_DOCUMENTATION.md
```

## How to Use

### 1. Creating a New API Controller

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CourtApp.Api.Controllers
{
    [Authorize]  // Require authentication
    public class CasesController : BaseController
    {
        [HttpGet("{id}")]
        public IActionResult GetCase(int id)
        {
            // Get authenticated user information
            var userId = GetUserId();
            var userContext = GetUserContext();
            
            // Business logic here
            var caseData = new { id, title = "Case Title" };
            
            // Return standardized response
            return SuccessResponse(caseData, "Case retrieved successfully");
        }
        
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult CreateCase([FromBody] CreateCaseRequest request)
        {
            // Check role
            if (!UserHasRole("Admin"))
            {
                return ForbiddenResponse("Only admins can create cases");
            }
            
            // Your logic
            return SuccessResponse(data, "Case created successfully", 201);
        }
    }
}
```

### 2. Using User Context in Services

```csharp
public class CaseService : ICaseService
{
    // Inject IHttpContextAccessor to access user context
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CaseService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public void CreateCase(CreateCaseRequest request)
    {
        var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;
        // Use userId in your business logic
    }
}
```

### 3. Role-Based Authorization

```csharp
// Require single role
[Authorize(Roles = "Admin")]

// Require any role (comma-separated)
[Authorize(Roles = "Admin,Manager")]

// Combine with policy
[Authorize(Policy = "AdminOnly")]

// Check in code
if (UserHasRole("Admin"))
{
    // Admin logic
}

// Check multiple roles
if (UserHasAnyRole("Admin", "Manager"))
{
    // Admin or Manager logic
}
```

### 4. Standardized Responses

```csharp
// Success with data
return SuccessResponse(data, "Success message", 200);

// Success without data
return SuccessResponse("Operation completed successfully");

// Validation error
var errors = new List<string> { "Field is required", "Invalid email" };
return ValidationErrorResponse(errors, "Validation failed");

// Not found
return NotFoundResponse("Case not found");

// Forbidden
return ForbiddenResponse("You don't have permission");

// Unauthorized
return UnauthorizedResponse("Token expired");

// Server error
return ErrorResponse("Database connection failed", 500);
```

## Installation & Configuration

### 1. **appsettings.json** - Add JWT Configuration

```json
{
  "JWTSettings": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "CourtApp",
    "Audience": "CourtAppUsers",
    "DurationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### 2. **Project Dependencies**
The following NuGet packages should be installed:
- `Microsoft.AspNetCore.Authentication.JwtBearer`
- `System.IdentityModel.Tokens.Jwt`
- `Microsoft.AspNetCore.ApiVersioning`
- `Swashbuckle.AspNetCore`

### 3. **Building the Project**
```bash
# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the project
dotnet run

# Run with watch mode
dotnet watch run
```

## API Flow Diagram

```
Client Request
    ↓
[Middleware] CORS Check → Auth Check
    ↓
[Controller] Inherits from BaseController
    ↓
[BaseController] Extracts UserContext from JWT
    ↓
[Your Logic] Access GetUserContext(), CheckRoles(), etc.
    ↓
[Response] Return ApiResponse<T> via SuccessResponse()
    ↓
Client receives standardized JSON response
```

## Testing with Swagger

1. Navigate to `https://localhost:5001/swagger`
2. Click "Try it out" on any endpoint
3. Enter request data
4. Click "Execute"
5. See standardized response

## Testing Endpoints

### 1. Register User
```bash
curl -X POST https://localhost:5001/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### 2. Login
```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "user@example.com",
    "password": "Password123!"
  }'
```

### 3. Get User Profile (with token)
```bash
curl -X GET https://localhost:5001/api/auth/profile \
  -H "Authorization: Bearer YOUR_JWT_TOKEN"
```

## Security Considerations

1. **HTTPS Only**: Always use HTTPS in production
2. **Token Storage**: Store tokens in secure httpOnly cookies or localStorage
3. **Token Expiry**: Tokens expire after configured duration (default: 60 minutes)
4. **Refresh Tokens**: Use refresh tokens to extend sessions without re-login
5. **CORS**: Configure CORS to allow only trusted origins
6. **Rate Limiting**: Consider implementing rate limiting on auth endpoints
7. **Input Validation**: Always validate user input on server side
8. **Logging**: Log authentication attempts and failures
9. **Account Lockout**: Implement account lockout after failed login attempts
10. **Password Policy**: Enforce strong password requirements

## Common Issues & Solutions

### Issue: "Token validation failed"
**Solution**: Check JWT settings in appsettings.json match the token generation settings.

### Issue: CORS errors
**Solution**: Check CORS policy in Program.cs allows your client domain.

### Issue: 401 Unauthorized
**Solution**: Ensure token is included in Authorization header: `Bearer {token}`

### Issue: Claims not found in token
**Solution**: Check if claims are added when generating JWT token in IdentityService.

## Next Steps

1. ✅ Implement authentication endpoints
2. ✅ Create base controller with user context
3. ✅ Set up standardized responses
4. ✅ Configure JWT authentication
5. ⭕ Implement refresh token rotation
6. ⭕ Add rate limiting
7. ⭕ Implement token blacklisting for logout
8. ⭕ Add audit logging for authentication
9. ⭕ Implement two-factor authentication (2FA)
10. ⭕ Add OAuth 2.0 integration (Google, GitHub, etc.)

## Related Files

- `CourtApp.Infrastructure/Identity/Services/IdentityService.cs` - Identity service implementation
- `CourtApp.Infrastructure/Identity/Models/ApplicationUser.cs` - User model
- `CourtApp.Application/Interfaces/IIdentityService.cs` - Identity service interface
- `CourtApp.Application/DTOs/Identity/TokenRequest.cs` - Login request DTO
- `CourtApp.Application/DTOs/Identity/TokenResponse.cs` - Login response DTO

## Support & Contribution

For questions or improvements, please:
1. Check the API documentation
2. Review the Example Controller
3. Check Swagger documentation at `/swagger`
4. Consult with the development team

---

**Last Updated**: 2024
**Version**: 1.0
**Status**: Production Ready
