# 📋 CourtApp API Authentication - Setup Checklist

## ✅ Pre-Implementation (Already Done)

- [x] Created BaseController with user context extraction
- [x] Created ApiResponse models (generic & non-generic)
- [x] Created UserContextInfo model
- [x] Created AuthController template with 11 endpoints
- [x] Created ExampleController with working examples
- [x] Updated Program.cs with configuration template
- [x] Created comprehensive API documentation
- [x] Created implementation guide
- [x] Created integration guide
- [x] Created quick reference guide
- [x] Verified build - NO ERRORS ✅

## 📌 IMMEDIATE NEXT STEPS (Do These First)

### Step 1: Add Project References
- [ ] Open `CourtApp.Api/CourtApp.Api.csproj`
- [ ] Add CourtApp.Application reference
- [ ] Add CourtApp.Infrastructure reference
- [ ] Add CourtApp.Domain reference
- [ ] Run: `dotnet restore`
- [ ] Verify: `dotnet build` (should succeed)

### Step 2: Install NuGet Packages
```bash
# Run these commands in CourtApp.Api directory
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 7.0.0
dotnet add package Microsoft.AspNetCore.ApiVersioning --version 4.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

- [ ] Package: Microsoft.AspNetCore.Authentication.JwtBearer
- [ ] Package: System.IdentityModel.Tokens.Jwt
- [ ] Package: Microsoft.AspNetCore.ApiVersioning
- [ ] Package: Swashbuckle.AspNetCore
- [ ] Run: `dotnet restore`
- [ ] Verify: `dotnet build` (should succeed)

### Step 3: Update appsettings.json
- [ ] Open `CourtApp.Api/appsettings.json`
- [ ] Add JWTSettings section
- [ ] Set strong JWT Key (32+ characters)
- [ ] Set Issuer to "CourtApp"
- [ ] Set Audience to "CourtAppUsers"
- [ ] Set DurationInMinutes to desired value
- [ ] Save the file

**Template:**
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

### Step 4: Update Program.cs
- [ ] Uncomment application and infrastructure service registrations
- [ ] Uncomment JWT configuration section
- [ ] Uncomment CORS policy configuration
- [ ] Uncomment authentication and authorization setup
- [ ] Uncomment Swagger/OpenAPI configuration
- [ ] Save the file
- [ ] Run: `dotnet build` (should succeed)

## 🔧 CONFIGURATION PHASE

### Step 5: Configure AuthController
- [ ] Open `CourtApp.Api/Controllers/AuthController.cs`
- [ ] Find the constructor with TODO comment
- [ ] Add parameter: `IIdentityService identityService`
- [ ] Initialize private field: `_identityService = identityService;`
- [ ] Implement Login method (replace TODO)
- [ ] Implement Register method (replace TODO)
- [ ] Implement RefreshToken method (replace TODO)
- [ ] Implement ConfirmEmail method (replace TODO)
- [ ] Implement ForgotPassword method (replace TODO)
- [ ] Implement ResetPassword method (replace TODO)
- [ ] Run: `dotnet build` (should succeed)

### Step 6: Configure CORS (if using frontend)
- [ ] In Program.cs, update CORS origins with your frontend URLs
- [ ] For local development: add `http://localhost:3000`, `http://localhost:4200`
- [ ] For production: add your actual domain
- [ ] Save and build

### Step 7: Configure HTTPS (Production)
- [ ] Generate development certificate: `dotnet dev-certs https`
- [ ] For production, obtain SSL certificate
- [ ] Configure in appsettings.Production.json
- [ ] Update Kestrel configuration if needed

## 🧪 TESTING PHASE

### Step 8: Run the Application
- [ ] Open terminal in CourtApp.Api directory
- [ ] Run: `dotnet run`
- [ ] Wait for: "Now listening on: https://localhost:5001"
- [ ] Open browser: `https://localhost:5001` (should see Swagger)

### Step 9: Test Swagger UI
- [ ] Navigate to: `https://localhost:5001/swagger/ui`
- [ ] Verify all endpoints are listed
- [ ] Verify BaseController endpoints work (login, me, etc.)
- [ ] Verify ExampleController endpoints are available
- [ ] Check that protected endpoints show lock icon

### Step 10: Test Authentication Flow
- [ ] Test POST `/api/auth/register`
  - [ ] Input valid registration data
  - [ ] Verify response format is correct
  - [ ] Check status, message, data fields
  
- [ ] Test POST `/api/auth/login`
  - [ ] Input registered user credentials
  - [ ] Verify JWT token is returned
  - [ ] Copy the token for next tests
  
- [ ] Test GET `/api/auth/me` (with token)
  - [ ] Click "Authorize" button in Swagger
  - [ ] Paste token: `Bearer {token}`
  - [ ] Execute endpoint
  - [ ] Verify user information is returned
  
- [ ] Test Protected Example Endpoints
  - [ ] Test `/api/example/user-context`
  - [ ] Test `/api/example/check-role/{role}`
  - [ ] Test `/api/example/user-info`
  - [ ] Verify all return correct user data

### Step 11: Test Authorization
- [ ] Test role-based endpoint: `/api/example/admin-only`
  - [ ] Without proper role: should return 403
  - [ ] With proper role: should return data
  
- [ ] Test error responses:
  - [ ] `/api/example/example-error` (500)
  - [ ] `/api/example/example-validation-error` (422)
  - [ ] Missing auth token: 401

## 🔐 SECURITY VERIFICATION

### Step 12: Verify Security Settings
- [ ] JWT Key is strong (32+ chars, random)
- [ ] HTTPS is enforced in production mode
- [ ] CORS is restricted to trusted origins
- [ ] Authentication middleware is properly configured
- [ ] Authorization policies are applied correctly
- [ ] Error responses don't leak sensitive info
- [ ] No secrets in code or logs
- [ ] Rate limiting considered

### Step 13: Test Token Expiration
- [ ] Set JWTSettings.DurationInMinutes to 1 (for testing)
- [ ] Run application
- [ ] Get token from login endpoint
- [ ] Wait 1+ minute
- [ ] Try to use token on protected endpoint
- [ ] Verify: 401 Unauthorized error
- [ ] Reset DurationInMinutes back to desired value (e.g., 60)

### Step 14: Test Token Refresh
- [ ] Implement token refresh if provided by IIdentityService
- [ ] Test POST `/api/auth/refresh-token`
- [ ] Verify new token is returned
- [ ] Verify old token still works
- [ ] Document refresh token flow

## 📊 INTEGRATION VERIFICATION

### Step 15: Verify Claims Extraction
- [ ] Login and get token
- [ ] Test GET `/api/auth/claims`
- [ ] Verify all claims are present:
  - [ ] sub (subject/username)
  - [ ] email
  - [ ] first_name
  - [ ] last_name
  - [ ] uid (user ID)
  - [ ] roles
  - [ ] ip

### Step 16: Verify Role Checking
- [ ] Test GET `/api/example/check-role/Admin`
  - [ ] Admin user: should return true
  - [ ] Non-admin user: should return false
  
- [ ] Test GET `/api/example/check-roles?roles=Admin&roles=Lawyer`
  - [ ] User with role: should return true for hasAnyRole
  - [ ] User without role: should return false

### Step 17: Document Custom Endpoints
- [ ] Create new controllers inheriting from BaseController
- [ ] Test user context extraction
- [ ] Test role checking
- [ ] Verify response format
- [ ] Document endpoints in Swagger

## 📈 MONITORING & LOGGING

### Step 18: Setup Logging
- [ ] Configure logging level in appsettings
- [ ] Verify authentication logs are captured
- [ ] Monitor failed login attempts
- [ ] Log token validation errors
- [ ] Track authorization failures

### Step 19: Setup Error Tracking
- [ ] Configure exception handling middleware
- [ ] Verify error responses are logged
- [ ] Monitor API response times
- [ ] Track 4xx and 5xx errors
- [ ] Set up alerts for critical errors

## 🚀 DEPLOYMENT PREPARATION

### Step 20: Production Configuration
- [ ] Create `appsettings.Production.json`
- [ ] Use environment variables for secrets
- [ ] Configure production connection string
- [ ] Set appropriate logging levels
- [ ] Disable debug information
- [ ] Enable HTTPS only
- [ ] Configure security headers

### Step 21: Database Migrations
- [ ] Ensure all migrations are applied
- [ ] Verify user tables exist
- [ ] Test user creation in database
- [ ] Verify authentication works end-to-end

### Step 22: Deployment Testing
- [ ] Test in staging environment
- [ ] Verify HTTPS certificate
- [ ] Test CORS with actual frontend domain
- [ ] Verify all endpoints accessible
- [ ] Test with real load
- [ ] Monitor performance

## 🎓 DOCUMENTATION & TRAINING

### Step 23: Team Documentation
- [ ] Share API_AUTHENTICATION_DOCUMENTATION.md with team
- [ ] Share QUICK_REFERENCE.md for quick lookups
- [ ] Conduct team training session
- [ ] Document any customizations
- [ ] Create internal guidelines

### Step 24: API Documentation
- [ ] Update project README with API info
- [ ] Document all custom endpoints
- [ ] Provide curl examples
- [ ] Document authentication flow
- [ ] Document error codes

## ✅ FINAL CHECKLIST

### Before Going Live
- [ ] All tests passing
- [ ] No compiler warnings
- [ ] No security vulnerabilities
- [ ] Performance acceptable (under 100ms response)
- [ ] Logging working correctly
- [ ] Error handling tested
- [ ] Documentation complete
- [ ] Team trained
- [ ] Backup procedures in place
- [ ] Rollback plan documented

### Post-Deployment
- [ ] Monitor for errors
- [ ] Collect performance metrics
- [ ] Gather user feedback
- [ ] Document any issues
- [ ] Plan improvements
- [ ] Update documentation
- [ ] Schedule follow-up meeting

## 📞 SUPPORT RESOURCES

### Documentation Files Available
- `API_AUTHENTICATION_DOCUMENTATION.md` - API reference
- `README_AUTHENTICATION.md` - Implementation guide
- `INTEGRATION_GUIDE.md` - Step-by-step setup
- `QUICK_REFERENCE.md` - Quick lookup
- `FINAL_SUMMARY.md` - Overview
- `ExampleController.cs` - Code examples

### Quick Links
- BaseController methods: See `BaseController.cs`
- Response types: See `ApiResponse.cs`
- User context: See `UserContextInfo.cs`
- Examples: See `ExampleController.cs`

---

## 🎯 Success Criteria Met When:

✅ All checkboxes above are checked
✅ Application builds without errors
✅ All endpoints respond correctly
✅ JWT authentication works
✅ Role checking works
✅ User context extracts correctly
✅ Error responses are standardized
✅ Documentation is complete
✅ Team is trained
✅ Performance is acceptable

---

**Checklist Version**: 1.0
**Last Updated**: January 2024
**Status**: Ready to Use

Start from "IMMEDIATE NEXT STEPS" ⬆️ and work your way down!
