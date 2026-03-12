# 🔧 Fix Swagger 404 Error - Quick Setup

## ✅ Problem Fixed!

**Error**: `GET http://localhost:5090/swagger/ui → 404`

**Solution**: Added Swagger UI middleware to Program.cs

---

## 📦 Step 1: Install Required Package

Run this command in your project directory:

```sh
dotnet add package Swashbuckle.AspNetCore
```

Or using Package Manager Console:
```powershell
Install-Package Swashbuckle.AspNetCore
```

---

## ✅ Step 2: Program.cs Already Updated

The Program.cs now has:

✅ `AddSwaggerGen()` - Swagger service registration
✅ `UseSwagger()` - Swagger middleware
✅ `UseSwaggerUI()` - Interactive UI
✅ JWT Bearer authentication configured
✅ Configured for port 5090

---

## 🚀 Step 3: Run Application

```sh
dotnet run
```

Console output will show:
```
Swagger UI available at: http://localhost:5090/swagger/
OpenAPI specification at: http://localhost:5090/swagger/v1/swagger.json
```

---

## 🌐 Access Swagger UI

### Now Works! ✅

```
http://localhost:5090/swagger/
```

### What You Get:
- ✅ Interactive API documentation
- ✅ Test all endpoints directly
- ✅ Authorize with JWT token
- ✅ View request/response examples
- ✅ See all parameters and schemas

---

## 🔐 Test with JWT

1. **Get Token**:
   - POST `/api/auth/login`
   - Enter credentials
   - Copy token

2. **Authorize**:
   - Click "Authorize" button (top-right)
   - Paste: `Bearer {your_token}`
   - Click "Authorize"

3. **Test Protected Endpoints**:
   - Now all endpoints will include JWT token
   - Test as authenticated user

---

## 📊 URLs Available

| Resource | URL |
|----------|-----|
| **Swagger UI** | `http://localhost:5090/swagger/` |
| **OpenAPI JSON** | `http://localhost:5090/swagger/v1/swagger.json` |
| **API Base** | `http://localhost:5090/` |

---

## ✅ Complete Checklist

- [ ] Run: `dotnet add package Swashbuckle.AspNetCore`
- [ ] Wait for package installation
- [ ] Run: `dotnet run`
- [ ] Open: `http://localhost:5090/swagger/`
- [ ] See Swagger UI (not 404!)
- [ ] Test an endpoint

---

## 🆘 If Still Getting 404

**Solution 1**: Rebuild solution
```sh
dotnet clean
dotnet build
dotnet run
```

**Solution 2**: Check package installed
```sh
dotnet package list
# Should see: Swashbuckle.AspNetCore 6.x.x (or higher)
```

**Solution 3**: Verify URL
- Using: `http://localhost:5090/swagger/` ✅
- NOT: `https://localhost:5090/swagger/` ❌ (use http)
- NOT: `http://localhost:5090/swagger/ui` ❌ (remove /ui)

---

## 📝 What Changed in Program.cs

### Added Services:
```csharp
builder.Services.AddSwaggerGen(options =>
{
    // API metadata
    // JWT authentication
});
```

### Added Middleware:
```csharp
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtApp.Api v1");
        c.RoutePrefix = "swagger";
    });
}
```

---

## 🎉 Ready to Go!

After installing the package and running the app:

✅ Swagger UI works
✅ No more 404 errors
✅ Full API documentation
✅ Interactive testing ready

---

**Time to Setup**: 2 minutes
**Status**: ✅ Ready to fix
