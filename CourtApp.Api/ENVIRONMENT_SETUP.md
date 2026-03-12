# CourtApp.Api - Quick Environment Setup

## 🚀 Quick Start (5 minutes)

### Development Setup

```bash
# 1. Set environment to Development
set ASPNETCORE_ENVIRONMENT=Development

# 2. Restore packages
dotnet restore

# 3. Run the application
dotnet run

# Done! Application runs on https://localhost:5001
```

---

## 📋 Environment Configuration Checklist

### ✅ Development Environment

```bash
# ✓ Set Environment Variable
set ASPNETCORE_ENVIRONMENT=Development

# ✓ Database (use local)
# Connection: localhost:5433

# ✓ File Upload (use Local)
# Files: UploadedFiles/

# ✓ JWT Duration
# 1440 minutes (24 hours)

# ✓ Logging
# Level: Debug
# Output: Console + File
```

**Status**: ✅ **Ready for Development**

---

### ✅ Staging Environment

```bash
# ✓ Set Environment Variable
set ASPNETCORE_ENVIRONMENT=Staging

# ✓ Update appsettings.Staging.json
{
  "ConnectionStrings": {
    "Postgres": "staging-server-connection"
  },
  "AllowedHosts": "api-staging.yourdomain.com"
}

# ✓ File Upload (use Google Drive)
# Verify service account key file

# ✓ JWT Duration
# 120 minutes

# ✓ Logging
# Level: Information
# Output: File only
```

**Status**: 🔧 **Requires Configuration**

---

### ✅ Production Environment

```bash
# ✓ Set Environment Variable (Permanently)
# Windows: SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production")
# Linux: export ASPNETCORE_ENVIRONMENT=Production

# ✓ Set Secrets (IMPORTANT!)
dotnet user-secrets set "JWTSettings:Key" "your-secret-key"
dotnet user-secrets set "ConnectionStrings:Postgres" "prod-connection"
dotnet user-secrets set "MailSettings:Password" "mail-app-password"
dotnet user-secrets set "WhatsAppSettings:ApiKey" "whatsapp-api-key"

# ✓ File Upload (use Google Drive)
# Verify production folder ID

# ✓ Logging
# Level: Warning (errors only)
# Output: File to /var/log/courtapp/

# ✓ Ensure Log Directory
# mkdir -p /var/log/courtapp
# chmod 755 /var/log/courtapp

# ✓ Run in Release mode
dotnet run --configuration Release
```

**Status**: 🔐 **Requires Secure Setup**

---

## 🔧 Configuration Files Included

| File | Purpose | Used In |
|------|---------|---------|
| `appsettings.json` | Base configuration | All environments |
| `appsettings.Development.json` | Dev overrides | Development only |
| `appsettings.Staging.json` | Staging overrides | Staging only |
| `appsettings.Production.json` | Prod overrides | Production only |

---

## 📊 Default Settings by Environment

| Setting | Development | Staging | Production |
|---------|-------------|---------|------------|
| **Log Level** | Debug | Information | Warning |
| **Database** | Local (localhost:5433) | Staging Server | Production Server |
| **File Upload** | Local | Google Drive | Google Drive |
| **JWT Duration** | 24 hours | 2 hours | 1 hour |
| **Error Details** | Full | Limited | None |
| **Console Logging** | Yes | No | No |
| **File Logging** | Yes | Yes (30 files) | Yes (90 files) |

---

## 🌍 Switch Environment

### Development → Staging
```bash
set ASPNETCORE_ENVIRONMENT=Staging
dotnet run
```

### Staging → Production
```bash
set ASPNETCORE_ENVIRONMENT=Production
dotnet run --configuration Release
```

### Check Current Environment
```bash
# In appsettings - look at environment name
# In logs - shows "Development" / "Staging" / "Production"
# In code - app.Environment.EnvironmentName
```

---

## 🔐 Secrets Management

### Development (Local)
```bash
# Safe to store in appsettings.Development.json
# Or in User Secrets:
dotnet user-secrets set "JWTSettings:Key" "dev-key"
```

### Staging (Less Sensitive)
```bash
# Store in appsettings.Staging.json
# Or in environment variables
```

### Production (SECURE)
```bash
# NEVER in appsettings.Production.json
# USE User Secrets or Environment Variables:

# Option 1: User Secrets (development machines only)
dotnet user-secrets set "JWTSettings:Key" "prod-key"

# Option 2: Environment Variables (recommended)
export JWTSettings__Key="prod-key"

# Option 3: .env file with dotenv loader
JWTSettings__Key=prod-key
```

---

## ✨ Features by Environment

### 🔵 Development
- ✅ Debug logging enabled
- ✅ Full error messages
- ✅ Local file uploads
- ✅ Hot reload support
- ✅ Quick token expiration (24 hours)
- ✅ Console + File logging

### 🟡 Staging
- ✅ Information-level logging
- ✅ Limited error messages
- ✅ Google Drive uploads
- ✅ Production-like database
- ✅ Standard token expiration (2 hours)
- ✅ File logging with 30-day retention

### 🔴 Production
- ✅ Warning-level logging only
- ✅ No sensitive error details
- ✅ Google Drive uploads
- ✅ Production database
- ✅ Short token expiration (1 hour)
- ✅ File logging with 90-day retention
- ✅ No console output

---

## 📝 Configuration Reference

### Database Connection Strings

**Development:**
```
server=localhost;port=5433;user id=postgres;password=12345;database=ldms_31122025
```

**Staging:**
```
server=staging-pg.yourdomain.com;port=5432;user id=appuser;password=staging_password;database=ldms_staging
```

**Production:**
```
server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod
```

### JWT Settings

**Development:**
- Duration: 1440 minutes (24 hours)
- Issuer: CourtApp
- Audience: CourtAppUsers

**Staging:**
- Duration: 120 minutes (2 hours)
- Issuer: CourtApp
- Audience: CourtAppUsers

**Production:**
- Duration: 60 minutes (1 hour)
- Issuer: CourtApp
- Audience: CourtAppUsers

---

## 🆘 Troubleshooting

### "Wrong environment loaded"
```bash
# Check current environment
echo %ASPNETCORE_ENVIRONMENT%  # Windows
echo $ASPNETCORE_ENVIRONMENT  # Linux/Mac

# If wrong, set correct one
set ASPNETCORE_ENVIRONMENT=Development
```

### "Connection string not found"
1. Verify environment variable is set
2. Check ConnectionStrings section in appropriate appsettings file
3. Verify key name matches: `Postgres` or `SqlServer`

### "Logging files not created"
1. Verify log directory exists
2. Check directory permissions
3. For Production: ensure `/var/log/courtapp` exists

### "File uploads failing"
1. Check `UploadSettings:Provider` value
2. For Local: verify `UploadedFiles` folder exists
3. For Google Drive: verify service account key file

---

## 📚 Detailed Documentation

For comprehensive configuration guide:
→ Read: `CONFIGURATION_GUIDE.md`

---

**Version**: 1.0
**Updated**: January 2024
**Environments**: Development, Staging, Production
