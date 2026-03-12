# 🔧 Database Configuration Quick Reference

## TL;DR (Too Long; Didn't Read)

**Don't hardcode database connection strings. Configure them per environment.**

---

## ⚡ Quick Setup (Choose your environment)

### Development (Local Testing)

**PostgreSQL:**
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "Postgres": "server=localhost;port=5432;user id=postgres;password=your_password;database=ldms_dev"
  },
  "DatabaseProvider": "Postgres"
}
```

**SQL Server:**
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "SqlServer": "Server=(localdb)\\MSSQLLocalDB;Database=ldms_dev;TrustServerCertificate=True"
  },
  "DatabaseProvider": "SqlServer"
}
```

**Run:**
```bash
set ASPNETCORE_ENVIRONMENT=Development
dotnet run
```

---

### Staging (Production-Like Testing)

**PostgreSQL:**
```json
// appsettings.Staging.json
{
  "ConnectionStrings": {
    "Postgres": "server=staging-db.yourdomain.com;port=5432;user id=staging_user;password=staging_password;database=ldms_staging"
  },
  "DatabaseProvider": "Postgres"
}
```

**Run:**
```bash
set ASPNETCORE_ENVIRONMENT=Staging
dotnet run
```

---

### Production (Live/Secure)

**⚠️ NEVER add connection string to config file!**

**Use Environment Variable:**
```bash
set ConnectionStrings__Postgres=server=prod-db.yourdomain.com;port=5432;user id=prod_user;password=SECURE_PASSWORD;database=ldms_prod
set ASPNETCORE_ENVIRONMENT=Production
dotnet run --configuration Release
```

---

## 📊 Connection String Formats

### PostgreSQL
```
server=HOST;port=5432;user id=USERNAME;password=PASSWORD;database=DATABASE_NAME
```

### SQL Server
```
Server=HOST;Database=DATABASE_NAME;User Id=USERNAME;Password=PASSWORD;TrustServerCertificate=True
```

### PostgreSQL with SSL
```
server=HOST;port=5432;user id=USERNAME;password=PASSWORD;database=DATABASE_NAME;SSL Mode=Require;
```

### SQL Server with Encryption
```
Server=HOST;Database=DATABASE_NAME;User Id=USERNAME;Password=PASSWORD;Encrypt=true;
```

---

## 🔐 3 Ways to Configure (Choose one per environment)

### 1️⃣ Edit appsettings File (Development/Staging)
```json
{
  "ConnectionStrings": {
    "Postgres": "your-connection-string"
  }
}
```

### 2️⃣ User Secrets (Development/Local)
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "your-connection-string"
```

### 3️⃣ Environment Variable (Production/Secure)
```bash
set ConnectionStrings__Postgres=your-connection-string
```

---

## 📝 Examples by Provider

### Local PostgreSQL (Development)
```
server=localhost;port=5432;user id=postgres;password=dev123;database=ldms_dev
```

### Remote PostgreSQL (Staging/Production)
```
server=db.company.com;port=5432;user id=appuser;password=SECURE_PASS;database=ldms_prod
```

### Local SQL Server (Development)
```
Server=(localdb)\MSSQLLocalDB;Database=ldms_dev;TrustServerCertificate=True
```

### Remote SQL Server (Staging/Production)
```
Server=sql.company.com;Database=ldms_prod;User Id=appuser;Password=SECURE_PASS;TrustServerCertificate=True
```

### Azure PostgreSQL
```
server=myserver.postgres.database.azure.com;port=5432;user id=username@myserver;password=PASSWORD;database=ldms;SSL Mode=Require;
```

### AWS RDS PostgreSQL
```
server=mydb.region.rds.amazonaws.com;port=5432;user id=admin;password=PASSWORD;database=ldms;SSL Mode=Require;
```

---

## ✅ Checklist

### Before Development
- [ ] Database server running
- [ ] Database exists
- [ ] Update appsettings.Development.json
- [ ] Test: `dotnet run`

### Before Staging
- [ ] Staging database ready
- [ ] Update appsettings.Staging.json
- [ ] Set environment variable
- [ ] Test: `dotnet run`

### Before Production
- [ ] Production database secured
- [ ] Set ASPNETCORE_ENVIRONMENT=Production
- [ ] Configure environment variable with connection string
- [ ] No connection string in appsettings.Production.json
- [ ] Test connection before deploying

---

## 🎯 Environment Variables (For Production)

### What is it?
Environment variables store configuration outside of code files. They're used by the application at runtime.

### How to set (Windows)?
```bash
# Temporary (current session only)
set ConnectionStrings__Postgres=your-connection-string

# Permanent (computer-wide)
setx ConnectionStrings__Postgres your-connection-string
```

### How to set (Linux/Mac)?
```bash
# Temporary
export ConnectionStrings__Postgres="your-connection-string"

# Permanent (add to ~/.bashrc or ~/.zshrc)
echo 'export ConnectionStrings__Postgres="your-connection-string"' >> ~/.bashrc
```

### How to set (Docker)?
```dockerfile
ENV ConnectionStrings__Postgres="your-connection-string"
```

### How to set (Docker Compose)?
```yaml
environment:
  - ConnectionStrings__Postgres=your-connection-string
```

---

## 🚨 Common Mistakes

### ❌ Hardcoding production password in config file
```json
// BAD! Never do this!
{
  "ConnectionStrings": {
    "Postgres": "server=prod;user id=user;password=ACTUAL_PASSWORD;database=ldms"
  }
}
```

### ✅ Use environment variable instead
```bash
set ConnectionStrings__Postgres=server=prod;user id=user;password=ACTUAL_PASSWORD;database=ldms
```

### ❌ Wrong environment variable name
```bash
set ConnectionStrings:Postgres=...  // Wrong! Use double underscore
```

### ✅ Correct format
```bash
set ConnectionStrings__Postgres=...  // Correct! Double underscore
```

### ❌ Committing .env file to repository
```bash
# Don't commit .env file
git add .env  // BAD!
```

### ✅ Add to .gitignore
```bash
# .gitignore
.env
.env.local
appsettings.*.json
```

---

## 🔍 Test Your Connection

### Option 1: Check Logs
Run application and look for successful database connection message

### Option 2: Temporary Endpoint (Development only)
```csharp
[HttpGet("test-db")]
public IActionResult TestDb()
{
    try
    {
        // Your database context
        _context.Database.OpenConnection();
        _context.Database.CloseConnection();
        return Ok("Database connection successful!");
    }
    catch (Exception ex)
    {
        return StatusCode(500, ex.Message);
    }
}
```

Visit: `https://localhost:5001/test-db`

---

## 📚 Full Documentation

For detailed information:
→ Read: `DATABASE_CONFIGURATION.md`

---

**Quick Reference Version**: 1.0
**Last Updated**: January 2024
**Key Rule**: Never hardcode database credentials. Use environment-specific configuration.
