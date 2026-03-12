# CourtApp.Api - Environment Configuration Summary

## ✅ What Has Been Configured

### 📁 Configuration Files Created/Updated

1. **appsettings.json** (Updated)
   - ✅ Base configuration for all environments
   - ✅ Database settings (Postgres & SqlServer)
   - ✅ JWT configuration
   - ✅ Mail settings (from CourtApp.Web)
   - ✅ WhatsApp integration settings
   - ✅ File upload/storage settings (GoogleDrive, Azure, Local)
   - ✅ Serilog logging configuration

2. **appsettings.Development.json** (Updated)
   - ✅ Development-specific overrides
   - ✅ Debug logging enabled
   - ✅ Local file storage
   - ✅ Extended JWT duration (1440 minutes)
   - ✅ Development database connection

3. **appsettings.Staging.json** (Created)
   - ✅ Staging-specific overrides
   - ✅ Information-level logging
   - ✅ Google Drive storage
   - ✅ Standard JWT duration (120 minutes)
   - ✅ Staging database connection
   - ✅ Host restrictions

4. **appsettings.Production.json** (Created)
   - ✅ Production-specific overrides
   - ✅ Warning-level logging only
   - ✅ Google Drive storage
   - ✅ Short JWT duration (60 minutes)
   - ✅ Production database connection
   - ✅ Secure log paths
   - ✅ No console output

5. **Program.cs** (Updated)
   - ✅ Environment detection
   - ✅ Configuration loading based on environment
   - ✅ Global exception handling
   - ✅ Proper service registration
   - ✅ Environment-aware startup logging

### 📖 Documentation Created

1. **ENVIRONMENT_SETUP.md** (Quick Start)
   - ✅ Quick 5-minute setup guide
   - ✅ Environment checklist for each stage
   - ✅ Configuration files reference
   - ✅ Default settings by environment
   - ✅ Environment switching instructions
   - ✅ Troubleshooting tips

2. **CONFIGURATION_GUIDE.md** (Comprehensive)
   - ✅ Detailed configuration overview
   - ✅ Environment hierarchy explanation
   - ✅ Setup instructions for each environment
   - ✅ Security checklist
   - ✅ Configuration value examples
   - ✅ Environment variables reference
   - ✅ Best practices by environment
   - ✅ Verification procedures
   - ✅ Troubleshooting guide

---

## 🎯 Configuration Hierarchy

```
appsettings.json (Base)
    ↓
appsettings.{ENVIRONMENT}.json (Override)
    ↓
User Secrets (Development only)
    ↓
Environment Variables (All environments)
```

---

## 🌍 Environments Supported

### 1. Development
```bash
# Environment Variable
ASPNETCORE_ENVIRONMENT=Development

# Key Features:
✓ Debug logging
✓ Local file uploads (UploadedFiles/)
✓ 24-hour JWT expiration
✓ Full error messages
✓ Console + File logging
✓ Local database (localhost:5433)
```

### 2. Staging
```bash
# Environment Variable
ASPNETCORE_ENVIRONMENT=Staging

# Key Features:
✓ Information-level logging
✓ Google Drive uploads
✓ 2-hour JWT expiration
✓ Limited error messages
✓ File logging only (30-day retention)
✓ Staging database
✓ Host restrictions
```

### 3. Production
```bash
# Environment Variable
ASPNETCORE_ENVIRONMENT=Production

# Key Features:
✓ Warning-level logging
✓ Google Drive uploads
✓ 1-hour JWT expiration
✓ Minimal error messages
✓ File logging only (90-day retention)
✓ Production database
✓ Secure log paths (/var/log/courtapp)
✓ No console output
```

---

## 📊 Configuration Comparison

| Feature | Development | Staging | Production |
|---------|-------------|---------|------------|
| **Log Level** | Debug | Information | Warning |
| **Database** | localhost:5433 | staging-server | prod-server |
| **File Upload** | Local | GoogleDrive | GoogleDrive |
| **JWT Duration** | 1440 min (24h) | 120 min (2h) | 60 min (1h) |
| **Error Details** | Full | Limited | None |
| **Console Log** | ✅ Yes | ❌ No | ❌ No |
| **File Log** | ✅ Yes | ✅ Yes (30 days) | ✅ Yes (90 days) |
| **Log Path** | Logs/ | Logs/ | /var/log/courtapp/ |

---

## 🔧 Quick Setup Commands

### Development
```bash
# Set environment
set ASPNETCORE_ENVIRONMENT=Development

# Run
dotnet run

# Application starts on https://localhost:5001
```

### Staging
```bash
# Set environment
set ASPNETCORE_ENVIRONMENT=Staging

# Update appsettings.Staging.json with staging values

# Run
dotnet run
```

### Production
```bash
# Set environment (permanent)
setx ASPNETCORE_ENVIRONMENT Production

# Set secrets
dotnet user-secrets set "JWTSettings:Key" "your-secret-key"
dotnet user-secrets set "ConnectionStrings:Postgres" "prod-connection-string"

# Run in Release mode
dotnet run --configuration Release
```

---

## 🔐 Security Configuration

### Development (Secure Enough)
- ✅ Local database with local credentials
- ✅ User Secrets for sensitive data
- ✅ Development-only appsettings

### Staging (Production-Like)
- ✅ Staging-specific connection strings
- ✅ Google Drive for files
- ✅ Information-level logging
- ✅ Production-like configuration

### Production (Maximum Security)
- ✅ User Secrets OR Environment Variables (NO appsettings secrets)
- ✅ HTTPS enforced
- ✅ Restricted host access
- ✅ Warning-level logging only
- ✅ No console output
- ✅ Secure log directory permissions
- ✅ Strong JWT key
- ✅ Secure database credentials

---

## 📝 Configuration Shared with CourtApp.Web

✅ **Shared from CourtApp.Web appsettings.json:**
- MailSettings (Gmail SMTP)
- WhatsAppSettings (360Dialog integration)
- UploadSettings (GoogleDrive, Azure, Local)
- ConnectionStrings (Postgres, SqlServer)
- DatabaseProvider selection
- Logging configuration

✅ **Additional for CourtApp.Api:**
- JWTSettings (authentication)
- Serilog configuration
- API-specific settings

---

## 🚀 Getting Started

### Step 1: Quick Overview
→ Read: `ENVIRONMENT_SETUP.md` (5 minutes)

### Step 2: Development Setup
```bash
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```
✅ Ready for development!

### Step 3: Staging Setup
→ Read: `CONFIGURATION_GUIDE.md` (Staging section)
→ Update: `appsettings.Staging.json`
→ Set: `ASPNETCORE_ENVIRONMENT=Staging`

### Step 4: Production Setup
→ Read: `CONFIGURATION_GUIDE.md` (Production section)
→ Set: Environment variables or User Secrets
→ Deploy with Release configuration

---

## 📚 Documentation Files

| File | Purpose | Read Time |
|------|---------|-----------|
| **ENVIRONMENT_SETUP.md** | Quick start guide | 5 min |
| **CONFIGURATION_GUIDE.md** | Comprehensive guide | 15 min |
| **appsettings.json** | Base configuration | - |
| **appsettings.Development.json** | Development overrides | - |
| **appsettings.Staging.json** | Staging overrides | - |
| **appsettings.Production.json** | Production overrides | - |

---

## 🎓 Key Concepts

### Configuration Inheritance
```
1. appsettings.json (loads first - base)
2. appsettings.{ENVIRONMENT}.json (overrides matching keys)
3. User Secrets / Environment Variables (override everything)
```

### Environment Detection
```
Automatic based on: ASPNETCORE_ENVIRONMENT variable
Default: Development (if not set)
```

### Configuration Access
```csharp
// In Program.cs or anywhere with IConfiguration
var databaseProvider = builder.Configuration["DatabaseProvider"];
var connectionString = builder.Configuration["ConnectionStrings:Postgres"];
var jwtKey = builder.Configuration["JWTSettings:Key"];
```

---

## ✅ Verification Checklist

### Development
- [ ] ASPNETCORE_ENVIRONMENT = Development
- [ ] appsettings.Development.json exists
- [ ] Database connection works
- [ ] Local file upload folder exists
- [ ] Application starts without errors

### Staging
- [ ] ASPNETCORE_ENVIRONMENT = Staging
- [ ] appsettings.Staging.json updated with staging values
- [ ] Staging database connection works
- [ ] Google Drive service account key configured
- [ ] Application starts without errors

### Production
- [ ] ASPNETCORE_ENVIRONMENT = Production (set permanently)
- [ ] All secrets configured via User Secrets or Environment Variables
- [ ] appsettings.Production.json does NOT contain secrets
- [ ] Production database connection works
- [ ] Log directory exists and has proper permissions
- [ ] HTTPS certificate configured
- [ ] Application builds in Release mode
- [ ] No errors on startup

---

## 🆘 Troubleshooting

### "Wrong configuration loaded"
1. Check environment variable: `echo %ASPNETCORE_ENVIRONMENT%`
2. Verify correct file: `appsettings.{ENVIRONMENT}.json`
3. Restart application after changing environment

### "Connection string error"
1. Verify database is running
2. Check ConnectionStrings in appropriate appsettings file
3. Verify credentials are correct
4. Test connection from local machine

### "Files not uploading"
1. Check UploadSettings:Provider value
2. For Local: verify folder exists
3. For GoogleDrive: verify service account key
4. For Azure: verify connection string

### "Logging not working"
1. Verify Serilog configuration in appsettings
2. For file logging: ensure Logs/ directory exists
3. For production: ensure /var/log/courtapp exists
4. Check file permissions

---

## 📞 Support

For environment-specific questions:
- Development issues → Read: ENVIRONMENT_SETUP.md (Dev section)
- Configuration details → Read: CONFIGURATION_GUIDE.md
- Secrets management → Read: CONFIGURATION_GUIDE.md (Security section)
- Troubleshooting → Read: CONFIGURATION_GUIDE.md (Troubleshooting section)

---

## 🎉 You Now Have

✅ Environment-aware configuration system
✅ Development, Staging, and Production setups
✅ Secure secrets management
✅ Comprehensive logging setup
✅ Database flexibility (Postgres/SqlServer)
✅ File upload options
✅ Email and WhatsApp integration settings
✅ Detailed documentation
✅ Step-by-step guides
✅ Security best practices

---

**Configuration Version**: 1.0
**Environments**: Development, Staging, Production
**Status**: ✅ **READY TO USE**
**Build Status**: ✅ **SUCCESS**

---

### 👉 Next Step: Read `ENVIRONMENT_SETUP.md` for quick start!
