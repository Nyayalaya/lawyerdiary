# CourtApp.Api - Environment Configuration Guide

## Overview

The CourtApp.Api uses environment-specific configuration files to manage different settings for Development, Staging, and Production environments.

---

## Configuration Files

### 1. **appsettings.json** (Base Configuration)
Default configuration used across all environments. Contains:
- Database connection strings (both Postgres and SqlServer)
- JWT settings
- Mail configuration
- WhatsApp integration settings
- File upload/storage settings
- Logging configuration
- Serilog configuration

**Usage**: Base settings inherited by all environments

---

### 2. **appsettings.Development.json** (Development)
Development-specific overrides:
- Debug logging enabled
- Local file storage for uploads
- Extended JWT token duration (1440 minutes / 24 hours)
- Debug-level Serilog configuration
- Development database connection

**Usage**: Automatically loaded when `ASPNETCORE_ENVIRONMENT=Development`

---

### 3. **appsettings.Staging.json** (Staging)
Staging-specific overrides:
- Information-level logging
- Google Drive storage for uploads
- Standard JWT token duration (120 minutes)
- Staging database connection
- Limited host access

**Usage**: Automatically loaded when `ASPNETCORE_ENVIRONMENT=Staging`

---

### 4. **appsettings.Production.json** (Production)
Production-specific overrides:
- Warning-level logging (errors only)
- No console output (file logging only)
- Google Drive storage for uploads
- Standard JWT token duration (60 minutes)
- Production database connection
- Restricted host access
- Secure log path (/var/log/courtapp)

**Usage**: Automatically loaded when `ASPNETCORE_ENVIRONMENT=Production`

---

## Environment-Specific Configuration Hierarchy

```
appsettings.json (Base)
    ↓
appsettings.{ENVIRONMENT}.json (Override)
    ↓
User Secrets (Development only)
    ↓
Environment Variables (Production)
```

---

## Setting Up Environments

### Development Environment

#### 1. Set Environment Variable
```bash
# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT = "Development"

# Windows (CMD)
set ASPNETCORE_ENVIRONMENT=Development

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Development
```

#### 2. Run Application
```bash
dotnet run

# Or with watch mode
dotnet watch run
```

#### 3. What to Expect
- Debug logging enabled
- Local file uploads (in `UploadedFiles` folder)
- Extended token duration
- Development database connection used
- Detailed error messages
- Full logging to console and file

---

### Staging Environment

#### 1. Set Environment Variable
```bash
# Windows (PowerShell)
$env:ASPNETCORE_ENVIRONMENT = "Staging"

# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Staging
```

#### 2. Update Configuration
Edit `appsettings.Staging.json`:
```json
{
  "ConnectionStrings": {
    "Postgres": "your-staging-connection-string"
  },
  "AllowedHosts": "api-staging.yourdomain.com"
}
```

#### 3. Run Application
```bash
dotnet run
```

#### 4. What to Expect
- Information-level logging
- Google Drive file uploads
- Standard token duration
- Staging database connection used
- Limited error messages
- Logging to file with retention

---

### Production Environment

#### 1. Set Environment Variable

**Using System Environment Variable:**
```bash
# Linux/Mac
export ASPNETCORE_ENVIRONMENT=Production

# Windows (PowerShell, permanent)
[Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", "Machine")
```

**Using Docker:**
```dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
```

**Using IIS Application Pool:**
- In IIS Manager, set environment variable in Application Pool settings

**Using systemd (Linux):**
```ini
# /etc/systemd/system/courtapp-api.service
[Service]
Environment="ASPNETCORE_ENVIRONMENT=Production"
```

#### 2. Secure Configuration (IMPORTANT!)

**Use User Secrets for Connection Strings:**
```bash
# Initialize user secrets (one time)
dotnet user-secrets init

# Set JWT Key (generate a strong key)
dotnet user-secrets set "JWTSettings:Key" "your-very-long-random-secret-key-minimum-32-chars"

# Set Database Connection
dotnet user-secrets set "ConnectionStrings:Postgres" "server=prod-server;user id=app;password=secure-password;database=ldms"

# Set Mail Password
dotnet user-secrets set "MailSettings:Password" "your-app-password"

# Set WhatsApp API Key
dotnet user-secrets set "WhatsAppSettings:ApiKey" "your-whatsapp-api-key"
```

**Use Environment Variables:**
```bash
export ConnectionStrings__Postgres="your-connection-string"
export JWTSettings__Key="your-secret-key"
export MailSettings__Password="your-mail-password"
export WhatsAppSettings__ApiKey="your-api-key"
```

**Or in .env file (with dotenv loader):**
```
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__Postgres=server=prod;...
JWTSettings__Key=very-secure-key
MailSettings__Password=secure-password
WhatsAppSettings__ApiKey=api-key
```

#### 3. Update Production Configuration
Edit `appsettings.Production.json`:
```json
{
  "AllowedHosts": "api.yourdomain.com,www.yourdomain.com",
  "ConnectionStrings": {
    "Postgres": "your-production-connection-string"
  },
  "UploadSettings": {
    "GoogleDrive": {
      "BaseFolderId": "your-production-folder-id"
    }
  }
}
```

#### 4. Ensure Log Directory Exists
```bash
# Linux/Mac
sudo mkdir -p /var/log/courtapp
sudo chown appuser:appuser /var/log/courtapp

# Or set a different path in appsettings.Production.json
```

#### 5. Run Application
```bash
dotnet run --configuration Release

# Or with systemd
sudo systemctl start courtapp-api
sudo systemctl enable courtapp-api

# Or with Docker
docker run -e ASPNETCORE_ENVIRONMENT=Production your-image
```

#### 6. What to Expect
- Warning-level logging only
- No console output
- File logging to `/var/log/courtapp/api-{date}.log`
- Google Drive file uploads
- Production database connection used
- Standard token duration
- Restricted host access
- Limited error messages (no sensitive info)

---

## Configuration Value Examples

### Development
```json
{
  "DatabaseProvider": "Postgres",
  "ConnectionStrings": {
    "Postgres": "server=localhost;port=5433;user id=postgres;password=12345;database=ldms_dev"
  },
  "JWTSettings": {
    "Key": "dev-secret-key-minimum-32-characters-long",
    "DurationInMinutes": 1440
  },
  "UploadSettings": {
    "Provider": "Local"
  }
}
```

### Staging
```json
{
  "DatabaseProvider": "Postgres",
  "ConnectionStrings": {
    "Postgres": "server=staging-server;user id=appuser;password=stagingpass;database=ldms_staging"
  },
  "JWTSettings": {
    "Key": "staging-secret-key-minimum-32-characters-long",
    "DurationInMinutes": 120
  },
  "UploadSettings": {
    "Provider": "GoogleDrive"
  },
  "AllowedHosts": "api-staging.yourdomain.com"
}
```

### Production
```json
{
  "DatabaseProvider": "Postgres",
  "ConnectionStrings": {
    "Postgres": "server=prod-server;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod"
  },
  "JWTSettings": {
    "Key": "USE_USER_SECRETS_OR_ENV_VAR",
    "DurationInMinutes": 60
  },
  "UploadSettings": {
    "Provider": "GoogleDrive"
  },
  "AllowedHosts": "api.yourdomain.com,www.yourdomain.com"
}
```

---

## Environment Variables Reference

All configuration values can be set via environment variables using double underscore (`__`) as separator:

```bash
# Database
ConnectionStrings__Postgres=server=...
ConnectionStrings__SqlServer=Server=...

# JWT
JWTSettings__Key=your-secret-key
JWTSettings__Issuer=CourtApp
JWTSettings__Audience=CourtAppUsers
JWTSettings__DurationInMinutes=60

# Mail
MailSettings__From=noreply@yourdomain.com
MailSettings__Host=smtp.gmail.com
MailSettings__Port=587
MailSettings__UserName=your-email@gmail.com
MailSettings__Password=your-app-password
MailSettings__DisplayName=Your Name

# WhatsApp
WhatsAppSettings__ApiUrl=https://waba.360dialog.io/v1/messages
WhatsAppSettings__ApiKey=your-api-key

# Upload
UploadSettings__Provider=GoogleDrive
UploadSettings__Folders__ProfileImages=profile-images
UploadSettings__Folders__DraftDocuments=draft-documents

# Logging
Logging__LogLevel__Default=Information
Serilog__MinimumLevel__Default=Warning
```

---

## Best Practices

### Development
✅ Use local storage for file uploads
✅ Enable debug logging
✅ Use local database
✅ Long JWT token duration
✅ Detailed error messages

### Staging
✅ Use same storage as production (Google Drive)
✅ Use production-like database
✅ Information-level logging
✅ Standard JWT duration
✅ Test file uploads
✅ Test mail sending

### Production
✅ Never commit secrets to repository
✅ Use User Secrets or Environment Variables
✅ Use cloud storage (Google Drive)
✅ Use secure database connection
✅ Warning-level logging only
✅ File logging to secure location
✅ Short JWT token duration
✅ Minimal error messages
✅ Monitor logs regularly
✅ Use HTTPS only

---

## Security Checklist

- [ ] JWT Key is strong (32+ characters, random)
- [ ] Connection strings use secure passwords
- [ ] Mail password is secure (not in appsettings.Production.json)
- [ ] WhatsApp API key is secure (not in appsettings.Production.json)
- [ ] ASPNETCORE_ENVIRONMENT is set correctly
- [ ] Logs directory exists and is secure
- [ ] HTTPS is enforced
- [ ] AllowedHosts is configured
- [ ] File permissions are restrictive
- [ ] Secrets are in User Secrets or Environment Variables

---

## Verifying Configuration

### Check Current Environment
```bash
# In code
var env = app.Environment.EnvironmentName; // Should be "Development", "Staging", or "Production"
```

### Check Configuration Values
Add this temporary endpoint in your controller:
```csharp
[HttpGet("config-check")]
public IActionResult ConfigCheck()
{
    return Ok(new
    {
        environment = app.Environment.EnvironmentName,
        databaseProvider = configuration["DatabaseProvider"],
        uploadProvider = configuration["UploadSettings:Provider"],
        jwtDuration = configuration["JWTSettings:DurationInMinutes"],
        allowedHosts = configuration["AllowedHosts"]
    });
}
```

Then access: `https://localhost:5001/api/config-check`

---

## Troubleshooting

### Configuration not loading
1. Check `ASPNETCORE_ENVIRONMENT` variable
2. Verify appsettings file exists
3. Check file naming: `appsettings.{ENVIRONMENT}.json`
4. Check JSON syntax is valid

### Wrong database connection used
1. Verify `ConnectionStrings:Postgres` value
2. Check `DatabaseProvider` setting
3. Verify credentials are correct
4. Test connection: `dotnet ef database update`

### Secrets not found
1. User Secrets only work in Development
2. For Production, use Environment Variables
3. Verify secret key names use double underscores

### File uploads failing
1. Verify `UploadSettings:Provider` value
2. For Local uploads: ensure folder exists and has permissions
3. For Google Drive: verify service account key file
4. For Azure: verify connection string

---

## Switching Environments

### From Development to Staging
```bash
# Update environment variable
$env:ASPNETCORE_ENVIRONMENT = "Staging"

# Update appsettings.Staging.json with staging values
# - Database connection
# - AllowedHosts

# Restart application
dotnet run
```

### From Staging to Production
```bash
# Update environment variable
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Set User Secrets or Environment Variables
dotnet user-secrets set "ConnectionStrings:Postgres" "production-string"
dotnet user-secrets set "JWTSettings:Key" "production-key"

# Deploy
dotnet publish -c Release
```

---

**Configuration Version**: 1.0
**Last Updated**: January 2024
**Environment Support**: Development, Staging, Production
