# CourtApp API - Integration & Configuration Guide

## 🔧 Step-by-Step Integration Guide

### Phase 1: Project References (5 minutes)

#### 1.1 Add Project References to CourtApp.Api.csproj

```xml
<ItemGroup>
  <ProjectReference Include="..\CourtApp.Application\CourtApp.Application.csproj" />
  <ProjectReference Include="..\CourtApp.Infrastructure\CourtApp.Infrastructure.csproj" />
  <ProjectReference Include="..\CourtApp.Domain\CourtApp.Domain.csproj" />
</ItemGroup>
```

#### 1.2 Verify References
```bash
# In CourtApp.Api directory
dotnet restore
```

---

### Phase 2: NuGet Packages (10 minutes)

#### 2.1 Install Required Packages

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add package System.IdentityModel.Tokens.Jwt --version 7.0.0
dotnet add package Microsoft.AspNetCore.ApiVersioning --version 4.0.0
dotnet add package Swashbuckle.AspNetCore --version 6.5.0
```

#### 2.2 Verify Installation
```bash
dotnet restore
dotnet build
```

---

### Phase 3: Configuration (15 minutes)

#### 3.1 Update appsettings.json

```json
{
  "JWTSettings": {
    "Key": "your-secret-key-must-be-at-least-32-characters-long!",
    "Issuer": "CourtApp",
    "Audience": "CourtAppUsers",
    "DurationInMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "your-connection-string-here"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

#### 3.2 Update appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Debug"
    }
  },
  "JWTSettings": {
    "Key": "your-dev-secret-key-must-be-at-least-32-characters-long!!",
    "DurationInMinutes": 1440
  }
}
```

---

### Phase 4: Update Program.cs (20 minutes)

Replace the entire Program.cs with:

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CourtApp.Application;
using CourtApp.Infrastructure;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure JWT Settings from appsettings
var jwtSettings = builder.Configuration.GetSection("JWTSettings");

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });

    options.AddPolicy("AllowSpecific", builder =>
    {
        builder.WithOrigins(
                "http://localhost:3000",      // React dev server
                "http://localhost:4200",      // Angular dev server
                "http://localhost:5173",      // Vite dev server
                "https://localhost:5001",     // HTTPS local
                "https://yourdomain.com"      // Production
            )
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Add Authentication with JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "default-key")
        ),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(10),
        RoleClaimType = "roles",
        NameClaimType = "sub"
    };

    // Log JWT validation errors
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        }
    };
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    // Add custom policies as needed
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin")
    );
    
    options.AddPolicy("LawyerOnly", policy =>
        policy.RequireRole("Lawyer")
    );

    options.AddPolicy("AdminOrLawyer", policy =>
        policy.RequireRole("Admin", "Lawyer")
    );
});

// Add Controllers
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressMapClientErrors = true;
    });

// Add API Versioning
builder.Services.AddApiVersioning(config =>
{
    config.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    config.AssumeDefaultVersionWhenUnspecified = true;
    config.ReportApiVersions = true;
});

// Add Swagger/OpenAPI
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CourtApp API",
        Version = "v1",
        Description = "Authentication and Case Management API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CourtApp Team",
            Email = "support@courtapp.com"
        }
    });

    // Add JWT Bearer Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme."
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });

    // Enable XML comments in Swagger
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Build the app
var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtApp API v1");
        c.RoutePrefix = "";
    });
}

app.UseHttpsRedirection();

// Use CORS before authentication
app.UseCors("AllowSpecific");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Global exception handling (optional)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.ContentType = "application/json";
        
        var exceptionFeature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var exception = exceptionFeature?.Error;

        var response = new
        {
            status = false,
            message = exception?.Message ?? "An unexpected error occurred",
            statusCode = context.Response.StatusCode,
            timestamp = DateTime.UtcNow
        };

        await context.Response.WriteAsJsonAsync(response);
    });
});

app.Run();
```

---

### Phase 5: Implement AuthController (30 minutes)

Update `CourtApp.Api/Controllers/AuthController.cs` with actual service calls:

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CourtApp.Application.Interfaces;
using CourtApp.Application.DTOs.Identity;
using CourtApp.Api.Models;

namespace CourtApp.Api.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] TokenRequest request)
        {
            try
            {
                var ipAddress = GetClientIpAddress();
                var result = await _identityService.GetTokenAsync(request, ipAddress);

                if (!result.Succeeded)
                    return FailureResponse(result.Message, 401);

                return SuccessResponse(result.Data, "Login successful", 200);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _identityService.RegisterAsync(request);

                if (!result.Succeeded)
                    return FailureResponse(result.Message, 400);

                return SuccessResponse(result.Data, 
                    "Registration successful. Please check your email to confirm.", 200);
            }
            catch (Exception ex)
            {
                return ErrorResponse(ex.Message, 500);
            }
        }

        // Add other methods similarly...
    }
}
```

---

### Phase 6: Testing (15 minutes)

#### 6.1 Build and Run
```bash
dotnet build
dotnet run
```

#### 6.2 Access Swagger
Navigate to: `https://localhost:5001/swagger/ui`

#### 6.3 Test Endpoints

**1. Register User**
```
POST /api/auth/register
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe",
  "userName": "johndoe"
}
```

**2. Login**
```
POST /api/auth/login
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Password123!"
}
```

**3. Get Profile (with token)**
```
GET /api/auth/profile
Authorization: Bearer {your_token_here}
```

---

### Phase 7: Verification Checklist

- [ ] Build succeeds without errors
- [ ] JWT settings configured in appsettings.json
- [ ] CORS policies updated
- [ ] Swagger UI accessible at root path
- [ ] Login endpoint returns JWT token
- [ ] Protected endpoints require authentication
- [ ] User context extracted correctly
- [ ] Roles and claims accessible
- [ ] Error responses in standard format
- [ ] HTTPS redirect working
- [ ] Logging shows authentication attempts

---

## 🚀 Production Deployment

### Security Checklist
- [ ] JWT Key is strong (32+ characters, random)
- [ ] HTTPS enforced in production
- [ ] CORS origins limited to trusted domains
- [ ] Token expiration set appropriately
- [ ] Implement token rotation
- [ ] Enable logging and monitoring
- [ ] Implement rate limiting
- [ ] Set up request throttling
- [ ] Enable request validation
- [ ] Configure OWASP headers

### appsettings.Production.json
```json
{
  "JWTSettings": {
    "Key": "use-environment-variables-for-secrets",
    "Issuer": "CourtApp",
    "Audience": "CourtAppUsers",
    "DurationInMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  }
}
```

### Use Environment Variables
```csharp
// In Program.cs
var jwtKey = builder.Configuration["JWTSettings:Key"] 
    ?? Environment.GetEnvironmentVariable("JWT_KEY")
    ?? throw new InvalidOperationException("JWT Key not configured");
```

---

## 📞 Support

For issues during integration:
1. Check IMPLEMENTATION_COMPLETE.md for overview
2. Review API_AUTHENTICATION_DOCUMENTATION.md for endpoint details
3. Check QUICK_REFERENCE.md for common patterns
4. Verify all NuGet packages are installed
5. Ensure project references are added
6. Check JWT settings in appsettings.json
7. Review error messages in Swagger UI
8. Check application logs

---

**Integration Guide Version 1.0**
**Last Updated: January 2024**
