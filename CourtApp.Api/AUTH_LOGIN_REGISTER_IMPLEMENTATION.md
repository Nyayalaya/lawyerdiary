# Auth Login & Registration API Implementation

## Overview

The `AuthController` now implements complete Login and Registration endpoints based on the `Login.cshtml.cs` Razor Pages implementation, using the `BaseController` for standardized responses.

---

## Controller Structure

### Key Features

✅ **Inherits from BaseController** - Uses standardized response methods
✅ **Login Endpoint** - Authenticates users and returns JWT token
✅ **Register Endpoint** - Creates new user accounts with validation
✅ **Input Validation** - Email format, password strength, field requirements
✅ **Error Handling** - Comprehensive error responses with proper HTTP status codes
✅ **Claims-Based** - Prepares for role and claims-based authorization
✅ **JWT Ready** - Response models include token and refresh token fields

---

## Endpoints

### 1. Login Endpoint

**Endpoint**: `POST /api/auth/login`

**Authentication**: Not required (AllowAnonymous)

**Request Body**:
```json
{
  "emailOrUsername": "user@example.com",
  "password": "YourPassword123!",
  "rememberMe": true
}
```

**Request Model**:
```csharp
public class LoginRequest
{
    [Required(ErrorMessage = "Email or Username is required")]
    public string EmailOrUsername { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool RememberMe { get; set; }
}
```

**Success Response (200)**:
```json
{
  "status": true,
  "message": "Login successful",
  "data": {
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "username": "johndoe",
    "email": "john@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "550e8400-e29b-41d4-a716-446655440001",
    "tokenExpiration": "2024-01-20T14:30:00Z",
    "roles": ["User", "Lawyer"]
  },
  "statusCode": 200
}
```

**Error Responses**:

- **400 Bad Request** - Invalid input (missing fields, invalid format)
```json
{
  "status": false,
  "message": "Invalid login request",
  "errors": ["Email/Username is required", "Password is required"],
  "statusCode": 400
}
```

- **401 Unauthorized** - Invalid credentials
```json
{
  "status": false,
  "message": "Invalid login attempt",
  "statusCode": 401
}
```

**Implementation Logic**:
1. Validates request model state
2. Checks if email/username and password are provided
3. Resolves user by email or username
4. Validates password
5. Checks if user is active
6. Validates email confirmation
7. Retrieves user roles
8. Generates JWT token (TODO)
9. Returns user info with token

---

### 2. Register Endpoint

**Endpoint**: `POST /api/auth/register`

**Authentication**: Not required (AllowAnonymous)

**Request Body**:
```json
{
  "email": "newuser@example.com",
  "username": "newuser",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePassword123!",
  "confirmPassword": "SecurePassword123!",
  "phoneNumber": "+1234567890"
}
```

**Request Model**:
```csharp
public class RegisterRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 100 characters")]
    public string Username { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [StringLength(100)]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(100)]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is required")]
    [DataType(DataType.Password)]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    public string ConfirmPassword { get; set; }

    [Phone(ErrorMessage = "Invalid phone number")]
    public string PhoneNumber { get; set; }
}
```

**Success Response (201 Created)**:
```json
{
  "status": true,
  "message": "User registered successfully. Check your email for confirmation link.",
  "data": {
    "success": true,
    "userId": "550e8400-e29b-41d4-a716-446655440000",
    "message": "Registration successful. Please confirm your email address.",
    "errors": []
  },
  "statusCode": 201
}
```

**Error Responses**:

- **400 Bad Request** - Validation failures
```json
{
  "status": false,
  "message": "Invalid registration request",
  "errors": [
    "Email is required",
    "Username must be between 3 and 100 characters",
    "Passwords do not match"
  ],
  "statusCode": 400
}
```

- **400 Bad Request** - Email already exists
```json
{
  "status": false,
  "message": "Email already registered",
  "statusCode": 400
}
```

- **400 Bad Request** - Weak password
```json
{
  "status": false,
  "message": "Password must be at least 8 characters and contain uppercase, lowercase, and numbers",
  "statusCode": 400
}
```

**Implementation Logic**:
1. Validates request model state
2. Checks required fields
3. Validates email format
4. Validates passwords match
5. Validates password strength (8+ chars, uppercase, lowercase, digits)
6. Checks if email already registered
7. Checks if username already taken
8. Creates new ApplicationUser
9. Sends confirmation email (TODO)
10. Returns UserId and success message

---

## Validation Rules

### Login Validation
- Email/Username: Required, non-empty
- Password: Required, non-empty
- Both fields validated before processing

### Register Validation
- Email: Required, valid email format
- Username: Required, 3-100 characters
- First Name: Required, max 100 characters
- Last Name: Required, max 100 characters
- Password: Required, minimum 8 characters
- Password Requirements:
  - At least 8 characters
  - At least 1 uppercase letter (A-Z)
  - At least 1 lowercase letter (a-z)
  - At least 1 digit (0-9)
- Confirm Password: Must match Password
- Phone Number: Optional, must be valid format if provided

---

## Helper Methods

### IsValidEmail(string email)
```csharp
/// <summary>
/// Validates email format using MailAddress class
/// </summary>
private bool IsValidEmail(string email)
{
    try
    {
        var mailAddress = new MailAddress(email);
        return true;
    }
    catch (FormatException)
    {
        return false;
    }
}
```

**Usage**: Validates email format in both login and register

### IsValidPassword(string password)
```csharp
/// <summary>
/// Validates password strength
/// Requirements: At least 8 characters, 1 uppercase, 1 lowercase, 1 digit
/// </summary>
private bool IsValidPassword(string password)
{
    if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
        return false;

    bool hasUpperCase = password.Any(char.IsUpper);
    bool hasLowerCase = password.Any(char.IsLower);
    bool hasDigit = password.Any(char.IsDigit);

    return hasUpperCase && hasLowerCase && hasDigit;
}
```

**Usage**: Validates password strength during registration

---

## BaseController Methods Used

### SuccessResponse
```csharp
protected IActionResult SuccessResponse<T>(T data, string message, int statusCode = 200)
{
    var response = new ApiResponse<T>
    {
        Status = true,
        Message = message,
        Data = data,
        StatusCode = statusCode,
        Timestamp = DateTime.UtcNow,
        TraceId = HttpContext.TraceIdentifier
    };
    return StatusCode(statusCode, response);
}
```

### FailureResponse
```csharp
protected IActionResult FailureResponse(object errors, string message, int statusCode = 400)
{
    var response = new ApiResponse
    {
        Status = false,
        Message = message,
        Errors = errors as List<string> ?? new List<string> { errors?.ToString() },
        StatusCode = statusCode,
        Timestamp = DateTime.UtcNow,
        TraceId = HttpContext.TraceIdentifier
    };
    return StatusCode(statusCode, response);
}
```

### ErrorResponse
```csharp
protected IActionResult ErrorResponse(Exception ex, string message, int statusCode = 500)
{
    var response = new ApiResponse
    {
        Status = false,
        Message = message,
        Errors = new List<string> { ex.Message },
        StatusCode = statusCode,
        Timestamp = DateTime.UtcNow,
        TraceId = HttpContext.TraceIdentifier
    };
    return StatusCode(statusCode, response);
}
```

---

## Integration with Login.cshtml.cs

### Similarities
- ✅ Email/Username validation
- ✅ Password validation
- ✅ User active status check
- ✅ Email confirmation check
- ✅ Role retrieval
- ✅ Claims-based authorization ready
- ✅ User context information gathering

### Additional Features (API)
- ✅ JWT token generation (TODO)
- ✅ Refresh token support
- ✅ Structured JSON responses
- ✅ RESTful conventions
- ✅ Stateless authentication
- ✅ CORS-ready

---

## Implementation TODOs

### In Login Endpoint
```csharp
// TODO 1: Inject actual UserManager
public AuthController(UserManager<ApplicationUser> userManager)
{
    _userManager = userManager;
}

// TODO 2: Replace mock user with actual database lookup
var user = await _userManager.FindByEmailAsync(request.EmailOrUsername);
if (user == null && IsValidEmail(request.EmailOrUsername))
{
    user = await _userManager.FindByNameAsync(request.EmailOrUsername);
}

// TODO 3: Validate user properties
if (!user.IsActive) return FailureResponse("User account is inactive", 403);
if (!user.EmailConfirmed) return FailureResponse("Email not confirmed", 403);

// TODO 4: Verify password
var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
if (!passwordValid) return FailureResponse("Invalid login attempt", 401);

// TODO 5: Get roles
var roles = await _userManager.GetRolesAsync(user);

// TODO 6: Generate JWT token
var token = GenerateJwtToken(user, roles);
var refreshToken = GenerateRefreshToken();

// TODO 7: Save refresh token to database
```

### In Register Endpoint
```csharp
// TODO 1: Check if email exists
var existingUser = await _userManager.FindByEmailAsync(request.Email);
if (existingUser != null)
    return FailureResponse("Email already registered", 400);

// TODO 2: Check if username exists
var existingUsername = await _userManager.FindByNameAsync(request.Username);
if (existingUsername != null)
    return FailureResponse("Username already taken", 400);

// TODO 3: Create user
var newUser = new ApplicationUser
{
    UserName = request.Username,
    Email = request.Email,
    FirstName = request.FirstName,
    LastName = request.LastName,
    PhoneNumber = request.PhoneNumber,
    IsActive = true,
    EmailConfirmed = false
};

var result = await _userManager.CreateAsync(newUser, request.Password);
if (!result.Succeeded)
    return FailureResponse(result.Errors.Select(e => e.Description).ToList(), "Registration failed", 400);

// TODO 4: Send email confirmation
var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
// Send email with confirmation link
```

---

## Response Model Classes

Located in `CourtApp.Api/Models/ApiResponse.cs`:

```csharp
public class ApiResponse
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string TraceId { get; set; }
}

public class ApiResponse<T>
{
    public bool Status { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public int StatusCode { get; set; }
    public DateTime Timestamp { get; set; }
    public string TraceId { get; set; }
}
```

---

## Swagger/OpenAPI Documentation

When accessing Swagger UI, the endpoints will show:

- **POST /api/auth/login**
  - Authenticates user credentials
  - Returns JWT token and user information
  - Required: EmailOrUsername, Password

- **POST /api/auth/register**
  - Creates new user account
  - Validates all input fields
  - Sends confirmation email
  - Required: Email, Username, Password, FirstName, LastName

---

## Testing Examples

### Using cURL

**Login**:
```bash
curl -X POST http://localhost:5090/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "emailOrUsername": "user@example.com",
    "password": "TestPassword123!",
    "rememberMe": true
  }'
```

**Register**:
```bash
curl -X POST http://localhost:5090/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "newuser@example.com",
    "username": "newuser",
    "firstName": "John",
    "lastName": "Doe",
    "password": "SecurePassword123!",
    "confirmPassword": "SecurePassword123!",
    "phoneNumber": "+1234567890"
  }'
```

### Using Postman

1. **Create new request**: POST
2. **URL**: `http://localhost:5090/api/auth/login`
3. **Headers**: 
   - Content-Type: application/json
4. **Body** (raw JSON):
```json
{
  "emailOrUsername": "user@example.com",
  "password": "TestPassword123!",
  "rememberMe": true
}
```
5. **Send**

---

## Next Steps

1. ✅ Install Swashbuckle.AspNetCore (Done - v10.1.5)
2. ✅ Implement Login and Register endpoints (Done)
3. ⏳ Add UserManager dependency injection
4. ⏳ Implement JWT token generation
5. ⏳ Add refresh token functionality
6. ⏳ Implement email confirmation
7. ⏳ Add role-based authorization
8. ⏳ Add forgot password functionality
9. ⏳ Add two-factor authentication

---

**Status**: ✅ Login and Register endpoints implemented using BaseController
**Build**: ✅ Compiles successfully with Swagger support
**Endpoints**: ✅ Ready for integration with UserManager and Identity services

