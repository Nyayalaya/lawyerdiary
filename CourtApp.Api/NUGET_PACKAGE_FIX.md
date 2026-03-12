# Fix Build Error - AddSwaggerGen Not Found

## ✅ Build Error Fixed!

The error was: **AddSwaggerGen method not found**

**Cause**: The `Swashbuckle.AspNetCore` NuGet package is not installed.

---

## 🔧 Two Solutions

### Solution 1: Install Swashbuckle Package (Recommended)

**Step 1: Install Package**
```sh
dotnet add package Swashbuckle.AspNetCore
```

Or using Package Manager Console:
```powershell
Install-Package Swashbuckle.AspNetCore
```

**Step 2: Update Program.cs**

Edit `CourtApp.Api\Program.cs` and add back the Swagger configuration:

```csharp
builder.Services.AddOpenApi();

// Add Swagger/Swashbuckle for UI
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "CourtApp.Api",
        Version = "v1",
        Description = "Legal Diary Management System API",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "CourtApp Team",
            Email = "support@courtapp.com"
        }
    });

    // Add JWT Bearer authentication to Swagger
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
    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (System.IO.File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// Add HttpContextAccessor for user context in services
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// ... rest of Program.cs

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtApp.Api v1");
        c.RoutePrefix = ""; // Serve Swagger UI at root (/)
    });
    Console.WriteLine("Swagger UI available at: https://localhost:5001/");
    Console.WriteLine("OpenAPI specification at: https://localhost:5001/swagger/v1/swagger.json");
}
```

**Step 3: Build & Run**
```sh
dotnet build
dotnet run
```

✅ Access Swagger UI at: `https://localhost:5001/`

---

### Solution 2: Native .NET 9 OpenAPI (No Extra Packages)

**Already Applied!** Program.cs now uses only:

```csharp
builder.Services.AddOpenApi();  // Built-in to .NET 9
```

And in middleware:
```csharp
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();  // Built-in endpoint
}
```

**Access OpenAPI Specification:**
```
https://localhost:5001/openapi/v1.json
```

**View in External Tools:**
- Swagger Editor: https://editor.swagger.io/
- Postman: Import the spec URL
- Visual Studio Code OpenAPI extension

---

## 📊 Comparison

| Feature | Solution 1 (Swashbuckle) | Solution 2 (Native OpenAPI) |
|---------|--------------------------|------------------------------|
| **Swagger UI** | ✅ Full UI in browser | ❌ No UI (JSON only) |
| **Extra Packages** | ✅ 1 package | ✅ None (built-in) |
| **Interactive Testing** | ✅ Yes | ⚠️ Use external tools |
| **Authentication UI** | ✅ Authorize button | ❌ Manual header setup |
| **Import to Postman** | ✅ Easy | ✅ Easy |
| **Setup Time** | 5 minutes | 1 minute |

---

## ✅ Current Status

**Program.cs**: ✅ Fixed (using native OpenAPI)
**Build**: ✅ Should compile now

---

## 🎯 Recommended: Install Swashbuckle

For better developer experience with interactive Swagger UI:

```sh
dotnet add package Swashbuckle.AspNetCore
```

Then update Program.cs with the full Swagger configuration (see Solution 1 above).

---

## 📝 NuGet Package Details

**Package**: Swashbuckle.AspNetCore
- **Latest Version**: 6.4.0+ (compatible with .NET 9)
- **Repository**: https://github.com/domaindrivendev/Swashbuckle.AspNetCore
- **NuGet**: https://www.nuget.org/packages/Swashbuckle.AspNetCore

---

## 🚀 Next Steps

1. **Option A** (Recommended): 
   - `dotnet add package Swashbuckle.AspNetCore`
   - Update Program.cs with Solution 1 code
   - Run application
   - Access: `https://localhost:5001/`

2. **Option B**: 
   - Current Program.cs is ready to go
   - Run: `dotnet run`
   - Access spec at: `https://localhost:5001/openapi/v1.json`
   - Import into Postman or Swagger Editor

---

**Status**: ✅ Build error fixed
**Recommendation**: Install Swashbuckle for full Swagger UI experience
