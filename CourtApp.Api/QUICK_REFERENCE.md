# CourtApp API - Quick Reference Guide

## 🚀 Quick Start

### 1. Standard Response in Any Controller
```csharp
public class YourController : BaseController
{
    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        return SuccessResponse(data, "Retrieved successfully");
    }
}
```

### 2. Get Authenticated User
```csharp
var userContext = GetUserContext();
var userId = GetUserId();
var userName = GetUserName();
var email = GetUserEmail();
```

### 3. Check User Roles
```csharp
if (UserHasRole("Admin")) { }
if (UserHasAnyRole("Admin", "Manager")) { }
if (UserHasAllRoles("Admin", "Lawyer")) { }
```

### 4. Handle Errors
```csharp
return FailureResponse("Invalid data", 400);
return UnauthorizedResponse();
return ForbiddenResponse();
return NotFoundResponse();
return ValidationErrorResponse(errors, "Invalid");
return ErrorResponse("Server error", 500);
```

---

## 📍 Response Types

### Success
```csharp
SuccessResponse<T>(data, "message", 200)
SuccessResponse("message", 200)
```

### Failure
```csharp
FailureResponse("message", 400, errors)
ErrorResponse("message", 500, errors)
UnauthorizedResponse("message")
ForbiddenResponse("message")
NotFoundResponse("message")
ValidationErrorResponse(errors, "message")
```

---

## 🔐 Authorization

### By Role
```csharp
[Authorize(Roles = "Admin")]
[Authorize(Roles = "Admin,Lawyer")]
```

### Programmatically
```csharp
if (!UserHasRole("Admin"))
    return ForbiddenResponse("Admin only");
```

---

## 📊 Response Format
```json
{
  "status": true,
  "message": "Success",
  "data": {},
  "statusCode": 200,
  "timestamp": "2024-01-15T10:30:00Z",
  "errors": []
}
```

---

## 🎯 UserContextInfo Properties
```csharp
userContext.UserId
userContext.UserName
userContext.Email
userContext.FirstName
userContext.LastName
userContext.FullName
userContext.Roles
userContext.Claims
userContext.IpAddress
userContext.IsAuthenticated
userContext.Mobile
userContext.Gender
userContext.DateOfBirth
```

---

## 🔗 Common Patterns

### Verify Authentication
```csharp
var user = GetUserContext();
if (!user.IsAuthenticated)
    return UnauthorizedResponse();
```

### Get Specific Claim
```csharp
var claimValue = GetUserContext().GetClaimValue("claim_type");
```

### Admin-Only Endpoint
```csharp
[Authorize(Roles = "Admin")]
[HttpPost]
public IActionResult AdminAction() { }
```

### Check Authorization in Code
```csharp
if (!UserHasRole("Admin"))
    return ForbiddenResponse("Admin access required");
```

---

## 📌 HTTP Status Codes

| Code | Method | Usage |
|------|--------|-------|
| 200 | SuccessResponse() | OK |
| 201 | SuccessResponse(data, "", 201) | Created |
| 400 | FailureResponse() | Bad Request |
| 401 | UnauthorizedResponse() | Unauthorized |
| 403 | ForbiddenResponse() | Forbidden |
| 404 | NotFoundResponse() | Not Found |
| 422 | ValidationErrorResponse() | Validation Error |
| 500 | ErrorResponse() | Server Error |

---

## 🏗️ File Structure

```
CourtApp.Api/
├── Controllers/
│   ├── BaseController.cs
│   ├── AuthController.cs
│   └── ExampleController.cs
├── Models/
│   ├── ApiResponse.cs
│   └── UserContextInfo.cs
├── Program.cs
└── Documentation/
    ├── API_AUTHENTICATION_DOCUMENTATION.md
    ├── README_AUTHENTICATION.md
    └── IMPLEMENTATION_COMPLETE.md
```

---

## ✅ Checklist Before Going Live

- [ ] Add project references
- [ ] Install NuGet packages
- [ ] Configure JWT settings
- [ ] Update Program.cs
- [ ] Test authentication endpoints
- [ ] Test authorization checks
- [ ] Verify CORS settings
- [ ] Check HTTPS configuration
- [ ] Review logging
- [ ] Test with Swagger

---

## 🐛 Troubleshooting

**Issue**: Claims not found
- Check JWT token generation in IdentityService
- Verify claim types match expected names

**Issue**: 401 Unauthorized
- Ensure token is in Authorization header
- Check token hasn't expired
- Verify token is valid

**Issue**: 403 Forbidden
- User doesn't have required role
- Check role assignment in database

**Issue**: CORS errors
- Update CORS policy in Program.cs
- Check allowed origins

---

## 📚 Important Files

1. **BaseController.cs** - All base functionality
2. **ApiResponse.cs** - Response wrapper
3. **UserContextInfo.cs** - User data model
4. **ExampleController.cs** - Usage examples
5. **API_AUTHENTICATION_DOCUMENTATION.md** - API reference

---

## 🎯 Common Tasks

### Get Current User ID
```csharp
string userId = GetUserId();
```

### Return Error Response
```csharp
return ErrorResponse("Error message", 500);
```

### Check if Admin
```csharp
if (UserHasRole("Admin")) { /* ... */ }
```

### Get All Claims
```csharp
var claims = GetUserContext().Claims;
```

### Return Data with Custom Message
```csharp
return SuccessResponse(data, "Custom message", 200);
```

---

**Quick Reference Version 1.0**
