# CourtApp API Authentication Documentation

## Overview

The CourtApp API provides a comprehensive authentication system with JWT token-based security. The API follows REST principles and returns standardized JSON responses for all endpoints.

## Features

- **JWT Authentication**: Secure token-based authentication
- **User Registration & Login**: Complete user management
- **Role-Based Access Control (RBAC)**: Control access based on user roles
- **Claim-Based Authorization**: Fine-grained permission control
- **Token Refresh**: Extend sessions with refresh tokens
- **Email Verification**: Confirm user email addresses
- **Password Management**: Forgot password and reset password functionality
- **Standardized Responses**: Consistent response format across all endpoints

## Standard Response Format

All API responses follow this standard format:

```json
{
  "status": true,
  "message": "Success message",
  "data": {},
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

### Response Fields

| Field | Type | Description |
|-------|------|-------------|
| `status` | boolean | `true` for success, `false` for failure |
| `message` | string | Human-readable response message |
| `data` | object | Response payload (varies by endpoint) |
| `statusCode` | integer | HTTP status code |
| `timestamp` | string | UTC timestamp of the response |
| `errors` | array | List of error messages (if any) |

## BaseController Features

The `BaseController` provides the following methods and properties to derived controllers:

### User Context Methods

```csharp
// Get complete user context information
UserContextInfo GetUserContext()

// Get specific user properties
string GetUserId()
string GetUserName()
string GetUserEmail()

// Check user roles
bool UserHasRole(string role)
bool UserHasAnyRole(params string[] roles)
bool UserHasAllRoles(params string[] roles)
```

### Response Helper Methods

```csharp
// Success responses
SuccessResponse<T>(T data, string message = "Success", int statusCode = 200)
SuccessResponse(string message = "Success", int statusCode = 200)

// Error responses
FailureResponse(string message, int statusCode = 400, List<string> errors = null)
ErrorResponse(string message, int statusCode = 500, List<string> errors = null)
UnauthorizedResponse(string message = "Unauthorized")
ForbiddenResponse(string message = "Forbidden")
NotFoundResponse(string message = "Not Found")
ValidationErrorResponse(List<string> errors, string message = "Validation failed")
```

## UserContextInfo Object

The `UserContextInfo` object contains all authenticated user information:

```csharp
public class UserContextInfo
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public List<string> Roles { get; set; }
    public List<Claim> Claims { get; set; }
    public string IpAddress { get; set; }
    public bool IsAuthenticated { get; set; }
    public string Mobile { get; set; }
    public string Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string ProfilePictureUrl { get; set; }
}
```

## Authentication Endpoints

### 1. User Login

**Endpoint:** `POST /api/auth/login`

**Description:** Authenticates user with email and password, returns JWT token.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Login successful",
  "data": {
    "id": "user-id-123",
    "jwToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "issuedOn": "2024-01-15T10:30:00Z",
    "expiresOn": "2024-01-15T11:30:00Z",
    "email": "user@example.com",
    "userName": "john_doe",
    "roles": ["User", "Lawyer"],
    "isVerified": true,
    "refreshToken": "refresh-token-123"
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

**Response (Failure - 401):**
```json
{
  "status": false,
  "message": "Invalid Credentials for 'user@example.com'.",
  "data": null,
  "statusCode": 401,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 2. User Registration

**Endpoint:** `POST /api/auth/register`

**Description:** Creates a new user account.

**Request:**
```json
{
  "email": "newuser@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "userName": "john_doe"
}
```

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Registration successful. Please check your email to confirm your account.",
  "data": {
    "id": "new-user-id",
    "email": "newuser@example.com",
    "userName": "john_doe",
    "firstName": "John",
    "lastName": "Doe"
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 3. Refresh Token

**Endpoint:** `POST /api/auth/refresh-token`

**Description:** Generates a new JWT token using a refresh token.

**Request:**
```json
{
  "token": "refresh-token-123"
}
```

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Token refreshed successfully",
  "data": {
    "id": "user-id-123",
    "jwToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "issuedOn": "2024-01-15T10:30:00Z",
    "expiresOn": "2024-01-15T11:30:00Z",
    "refreshToken": "new-refresh-token"
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 4. Confirm Email

**Endpoint:** `POST /api/auth/confirm-email`

**Query Parameters:**
- `userId` (required): User ID
- `code` (required): Email confirmation code (from email)

**Example:** `POST /api/auth/confirm-email?userId=user-123&code=confirmation-code-123`

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Email confirmed successfully",
  "data": null,
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 5. Forgot Password

**Endpoint:** `POST /api/auth/forgot-password`

**Description:** Sends a password reset link to the user's email.

**Request:**
```json
{
  "email": "user@example.com"
}
```

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Password reset link has been sent to your email",
  "data": null,
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 6. Reset Password

**Endpoint:** `POST /api/auth/reset-password`

**Description:** Resets user password using the reset token.

**Request:**
```json
{
  "email": "user@example.com",
  "password": "NewPassword123!",
  "confirmPassword": "NewPassword123!",
  "token": "reset-token-from-email"
}
```

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Password reset successfully",
  "data": null,
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 7. Get User Profile

**Endpoint:** `GET /api/auth/profile`

**Authentication:** Required (Bearer Token)

**Description:** Returns complete authenticated user information and claims.

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "User profile retrieved successfully",
  "data": {
    "userId": "user-id-123",
    "userName": "john_doe",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "roles": ["User", "Lawyer"],
    "claims": [
      {
        "type": "sub",
        "value": "john_doe"
      },
      {
        "type": "email",
        "value": "user@example.com"
      }
    ],
    "ipAddress": "192.168.1.1",
    "isAuthenticated": true,
    "mobile": "+91-9876543210",
    "gender": "Male",
    "dateOfBirth": "1990-01-15T00:00:00Z"
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 8. Get Current User (Simplified)

**Endpoint:** `GET /api/auth/me`

**Authentication:** Required (Bearer Token)

**Description:** Returns simplified current user information.

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "User information retrieved successfully",
  "data": {
    "userId": "user-id-123",
    "userName": "john_doe",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "fullName": "John Doe",
    "roles": ["User", "Lawyer"],
    "mobile": "+91-9876543210",
    "gender": "Male",
    "dateOfBirth": "1990-01-15T00:00:00Z",
    "ipAddress": "192.168.1.1"
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 9. Get User Roles

**Endpoint:** `GET /api/auth/roles`

**Authentication:** Required (Bearer Token)

**Description:** Returns list of roles assigned to the user.

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "User roles retrieved successfully",
  "data": ["User", "Lawyer", "Admin"],
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 10. Get User Claims

**Endpoint:** `GET /api/auth/claims`

**Authentication:** Required (Bearer Token)

**Description:** Returns all claims associated with the user.

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "User claims retrieved successfully",
  "data": [
    {
      "type": "sub",
      "value": "john_doe"
    },
    {
      "type": "email",
      "value": "user@example.com"
    },
    {
      "type": "first_name",
      "value": "John"
    },
    {
      "type": "roles",
      "value": "Lawyer"
    }
  ],
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

### 11. Verify Token

**Endpoint:** `POST /api/auth/verify-token`

**Authentication:** Required (Bearer Token)

**Description:** Validates the JWT token and returns user information.

**Response (Success - 200):**
```json
{
  "status": true,
  "message": "Token is valid",
  "data": {
    "isValid": true,
    "userId": "user-id-123",
    "userName": "john_doe",
    "email": "user@example.com",
    "roles": ["User", "Lawyer"]
  },
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

## JWT Token Header

Include the JWT token in the Authorization header:

```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

## HTTP Status Codes

| Code | Meaning | Usage |
|------|---------|-------|
| 200 | OK | Successful request |
| 400 | Bad Request | Invalid input or validation error |
| 401 | Unauthorized | Invalid credentials or missing token |
| 403 | Forbidden | User lacks required permissions |
| 404 | Not Found | Resource not found |
| 422 | Unprocessable Entity | Validation error |
| 500 | Internal Server Error | Server error |

## Example Usage in Controllers

### Basic Usage

```csharp
[Authorize]
public class CaseController : BaseController
{
    [HttpGet("{id}")]
    public IActionResult GetCase(int id)
    {
        var userId = GetUserId();
        var userContext = GetUserContext();
        
        // Your business logic here
        
        return SuccessResponse(data, "Case retrieved successfully");
    }
}
```

### Role-Based Access Control

```csharp
[Authorize(Roles = "Admin,Lawyer")]
[HttpPost]
public IActionResult CreateCase([FromBody] CreateCaseRequest request)
{
    if (!UserHasRole("Admin") && !UserHasRole("Lawyer"))
    {
        return ForbiddenResponse("Only Admins and Lawyers can create cases");
    }
    
    // Your business logic
    return SuccessResponse(data, "Case created successfully", 201);
}
```

### Claim-Based Authorization

```csharp
[Authorize]
[HttpPut("{id}")]
public IActionResult UpdateCase(int id, [FromBody] UpdateCaseRequest request)
{
    var userContext = GetUserContext();
    var createdBy = userContext.GetClaimValue("created_by");
    
    if (createdBy != id.ToString())
    {
        return ForbiddenResponse("You can only update your own cases");
    }
    
    // Your business logic
    return SuccessResponse(data, "Case updated successfully");
}
```

## Swagger Integration

The API includes Swagger/OpenAPI documentation. Access it at:
- **Development:** `https://localhost:5001/swagger/ui`
- **Production:** `https://your-api.com/swagger/ui`

All endpoints are documented with descriptions, request/response examples, and authentication requirements.

## Security Best Practices

1. **Always use HTTPS** in production
2. **Store tokens securely** in httpOnly cookies or secure storage
3. **Validate all inputs** on the server side
4. **Use strong passwords** with complexity requirements
5. **Implement rate limiting** to prevent abuse
6. **Keep tokens short-lived** (e.g., 1 hour expiry)
7. **Use refresh tokens** for session extension
8. **Implement token blacklisting** for logout functionality
9. **Monitor failed login attempts** and implement account lockouts
10. **Use CORS policies** to restrict cross-origin requests

## Configuration

JWT settings should be configured in `appsettings.json`:

```json
{
  "JWTSettings": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "CourtApp",
    "Audience": "CourtAppUsers",
    "DurationInMinutes": 60
  }
}
```

## Error Handling

All errors are returned in the standard format:

```json
{
  "status": false,
  "message": "Error message",
  "data": null,
  "statusCode": 400,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": [
    "Validation error 1",
    "Validation error 2"
  ]
}
```

## Support

For issues or questions, please contact the development team or create an issue in the repository.
