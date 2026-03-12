# CourtApp.Api - Swagger/OpenAPI Documentation Setup

## ✅ Swagger UI is Now Enabled!

The CourtApp.Api now has **Swagger/Swashbuckle** configured for both interactive API documentation and OpenAPI specification.

---

## 🌐 Access Swagger UI

### In Development Environment

After running `dotnet run`, access Swagger UI at:

```
https://localhost:5001/
```

Or:
```
https://localhost:5001/swagger/ui
```

---

## 📋 What You Get

### 1. **Swagger UI** (Interactive API Explorer)
- URL: `https://localhost:5001/`
- Features:
  - View all API endpoints
  - Test endpoints directly
  - See request/response examples
  - Authorize with JWT token
  - View endpoint documentation

### 2. **OpenAPI Specification**
- URL: `https://localhost:5001/swagger/v1/swagger.json`
- Features:
  - Machine-readable API specification
  - Can be imported into other tools
  - Used by code generators
  - Integrates with API testing tools

### 3. **JWT Authorization Support**
- Built-in "Authorize" button in Swagger UI
- Supports Bearer token authentication
- Scope support for fine-grained permissions

---

## 🔧 Features Configured

✅ **API Information**
- Title: CourtApp.Api
- Version: v1
- Description: Legal Diary Management System API
- Contact information

✅ **JWT Bearer Authentication**
- Security scheme for Bearer tokens
- Configured in all protected endpoints
- Automatic authorization header injection

✅ **XML Documentation**
- Reads XML comments from code
- Displays in Swagger UI
- Auto-generated from code documentation

✅ **Schema Definitions**
- Automatic model documentation
- Request/response body examples
- Parameter descriptions

---

## 🎯 Using Swagger UI

### Step 1: Start the Application
```bash
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

You'll see:
```
CourtApp.Api starting in Development environment
Swagger UI available at: https://localhost:5001/
OpenAPI specification at: https://localhost:5001/swagger/v1/swagger.json
```

### Step 2: Open Browser
Navigate to:
```
https://localhost:5001/
```

### Step 3: Authorize (For Protected Endpoints)
1. Click "Authorize" button (top-right)
2. Paste your JWT token: `Bearer {your_token_here}`
3. Click "Authorize"
4. Now you can test protected endpoints

### Step 4: Test an Endpoint
1. Click on any endpoint to expand it
2. Click "Try it out"
3. Fill in required parameters
4. Click "Execute"
5. View the response

---

## 📝 Example Endpoints Visible

### Public Endpoints (No Auth Required)
- POST /api/auth/login
- POST /api/auth/register
- POST /api/auth/refresh-token

### Protected Endpoints (Auth Required)
- GET /api/auth/profile
- GET /api/auth/me
- GET /api/auth/roles
- GET /api/auth/claims

### Example Endpoints
- GET /api/example/user-context
- GET /api/example/check-role/{role}
- GET /api/example/user-info

---

## 🔐 JWT Authorization in Swagger

### Getting a Token

1. **Test** POST `/api/auth/login` endpoint:
   ```json
   {
     "email": "test@example.com",
     "password": "Password123!"
   }
   ```

2. **Copy** the JWT token from response

3. **Click** "Authorize" button

4. **Paste** token with "Bearer " prefix:
   ```
   Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
   ```

5. **Click** "Authorize"

6. **Now** all protected endpoints will include the token automatically

---

## 📋 Environment-Specific Behavior

### Development
✅ Swagger UI **ENABLED**
✅ Full API documentation
✅ Test all endpoints
✅ See detailed responses

### Staging
⚠️ Swagger UI **DISABLED** (can be enabled)
⚠️ Use production-like environment

### Production
❌ Swagger UI **DISABLED**
❌ For security (expose less information)
❌ Only OpenAPI spec available if needed

---

## 🔧 Customize Swagger UI

### Change Swagger UI Route

To serve at `/api/docs` instead of `/`:

Edit `Program.cs`:
```csharp
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtApp.Api v1");
    c.RoutePrefix = "api/docs"; // Changed from ""
});
```

Access at: `https://localhost:5001/api/docs`

### Add Multiple API Versions

```csharp
options.SwaggerDoc("v2", new Microsoft.OpenApi.Models.OpenApiInfo
{
    Title = "CourtApp.Api",
    Version = "v2"
});

// In UseSwaggerUI:
c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtApp.Api v1");
c.SwaggerEndpoint("/swagger/v2/swagger.json", "CourtApp.Api v2");
```

---

## 📚 Adding Documentation to Code

### XML Comments on Controllers

```csharp
/// <summary>
/// Get user profile information
/// </summary>
/// <returns>Returns the authenticated user's profile</returns>
[HttpGet("profile")]
public IActionResult GetProfile()
{
    var userContext = GetUserContext();
    return SuccessResponse(userContext, "Profile retrieved");
}
```

### XML Comments on Parameters

```csharp
/// <summary>
/// Check if user has a specific role
/// </summary>
/// <param name="role">The role to check (e.g., "Admin", "Lawyer")</param>
/// <returns>Returns role check result</returns>
[HttpGet("check-role/{role}")]
public IActionResult CheckRole(string role)
{
    var hasRole = UserHasRole(role);
    return SuccessResponse(new { role, hasRole });
}
```

---

## 🔍 Troubleshooting

### "Swagger UI shows 404"
**Solution:**
1. Verify you're accessing: `https://localhost:5001/`
2. Check environment is "Development"
3. Restart application: `dotnet run`

### "OpenAPI endpoint not found"
**Solution:**
1. Verify endpoint: `https://localhost:5001/swagger/v1/swagger.json`
2. Ensure `app.UseSwagger()` is called
3. Check `Program.cs` has `AddSwaggerGen()`

### "Swagger UI is empty (no endpoints)"
**Solution:**
1. Verify controllers have `[ApiController]` attribute
2. Verify action methods have HTTP verb attributes: `[HttpGet]`, `[HttpPost]`, etc.
3. Ensure `builder.Services.AddControllers()` is called
4. Restart application

### "JWT Authorization not working"
**Solution:**
1. Verify token is valid: `Bearer {token}`
2. Ensure token includes proper claims
3. Check endpoint has `[Authorize]` attribute
4. Verify Bearer scheme is configured in Swagger

### "Models not showing in Swagger"
**Solution:**
1. Use request/response models in endpoints
2. Add XML comments to model properties
3. Ensure models are public classes
4. Rebuild solution

---

## 🚀 Enable/Disable by Environment

### Development (Enabled)
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);
}
```

### All Environments (Always Enabled)
```csharp
// Remove if statement
app.UseSwagger();
app.UseSwaggerUI(...);
```

### Conditional (Custom Logic)
```csharp
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(...);
}
```

---

## 📊 Swagger vs OpenAPI

### Swagger UI
- **What**: Interactive web interface
- **URL**: `https://localhost:5001/`
- **For**: Developers testing APIs
- **Access**: Browser
- **Format**: HTML/JavaScript

### OpenAPI Specification
- **What**: Machine-readable specification
- **URL**: `https://localhost:5001/swagger/v1/swagger.json`
- **For**: Code generators, API tools
- **Access**: Programmatic
- **Format**: JSON

---

## 🔗 External Tools Integration

You can import the OpenAPI spec into:

### Postman
1. Import: `https://localhost:5001/swagger/v1/swagger.json`
2. Automatically creates collection
3. Ready for testing

### Visual Studio Code
1. Install OpenAPI extension
2. Open spec: `https://localhost:5001/swagger/v1/swagger.json`
3. View and test endpoints

### Code Generators
1. Use spec URL for automatic client generation
2. Supports multiple languages (C#, TypeScript, Python, etc.)

---

## ✅ Verification Checklist

- [ ] Run application in Development
- [ ] Visit `https://localhost:5001/`
- [ ] Swagger UI loads without 404
- [ ] See list of endpoints
- [ ] Can expand and view endpoint details
- [ ] "Authorize" button visible
- [ ] Can test a public endpoint
- [ ] OpenAPI spec accessible at `/swagger/v1/swagger.json`

---

## 📞 Support

### Documentation Issues
- Check: XML comments in code
- Verify: Models are used in responses
- Enable: Include XML comments in project

### Authorization Issues
- Verify: Token is valid
- Check: `[Authorize]` attributes on methods
- Test: Public endpoints first

### Display Issues
- Clear: Browser cache
- Try: Different browser
- Restart: Application

---

## 🎯 Next Steps

1. ✅ Run: `dotnet run`
2. ✅ Open: `https://localhost:5001/`
3. ✅ Explore: API endpoints
4. ✅ Test: Public endpoints
5. ✅ Add: XML documentation to your code
6. ✅ Test: Protected endpoints with JWT

---

**Swagger Setup Version**: 1.0
**Status**: ✅ **ENABLED IN DEVELOPMENT**
**Access Point**: `https://localhost:5001/`
