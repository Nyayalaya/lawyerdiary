# Database Configuration Guide - CourtApp.Api

## 🔒 Important: Database Connection Strings

**Connection strings are NOT hardcoded in configuration files.** Each environment uses its own database configuration that must be provided at runtime.

---

## 📋 Configuration by Environment

### Development Environment

**Where to configure:** `appsettings.Development.json` or User Secrets

```json
{
  "ConnectionStrings": {
    "Postgres": "YOUR_DEVELOPMENT_CONNECTION_STRING",
    "SqlServer": "YOUR_DEVELOPMENT_CONNECTION_STRING"
  },
  "DatabaseProvider": "Postgres"
}
```

**Example Connection Strings:**

**PostgreSQL (Development):**
```
server=localhost;port=5433;user id=postgres;password=your_password;database=ldms_dev
```

**SQL Server (Development):**
```
Server=(localdb)\MSSQLLocalDB;Database=ldms_dev;TrustServerCertificate=True
```

**How to configure:**

Option 1: Edit `appsettings.Development.json`
```json
{
  "ConnectionStrings": {
    "Postgres": "server=localhost;port=5433;user id=postgres;password=your_password;database=ldms_dev"
  }
}
```

Option 2: Use User Secrets (Recommended)
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Postgres" "server=localhost;port=5433;user id=postgres;password=your_password;database=ldms_dev"
```

---

### Staging Environment

**Where to configure:** `appsettings.Staging.json` or User Secrets

```json
{
  "ConnectionStrings": {
    "Postgres": "YOUR_STAGING_CONNECTION_STRING",
    "SqlServer": "YOUR_STAGING_CONNECTION_STRING"
  },
  "DatabaseProvider": "Postgres"
}
```

**Example Connection Strings:**

**PostgreSQL (Staging):**
```
server=staging-pg.yourdomain.com;port=5432;user id=appuser;password=staging_secure_password;database=ldms_staging
```

**SQL Server (Staging):**
```
Server=staging-sql.yourdomain.com;Database=ldms_staging;User Id=appuser;Password=staging_secure_password;TrustServerCertificate=True
```

**How to configure:**

Option 1: Edit `appsettings.Staging.json`
```json
{
  "ConnectionStrings": {
    "Postgres": "server=staging-pg.yourdomain.com;port=5432;user id=appuser;password=staging_password;database=ldms_staging"
  }
}
```

Option 2: Use Environment Variables
```bash
set ConnectionStrings__Postgres=server=staging-pg.yourdomain.com;port=5432;user id=appuser;password=staging_password;database=ldms_staging
```

---

### Production Environment

**Where to configure:** Environment Variables or User Secrets ONLY (NOT in appsettings.Production.json)

**⚠️ NEVER put secrets in appsettings.Production.json**

**Example Connection Strings:**

**PostgreSQL (Production):**
```
server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PRODUCTION_PASSWORD;database=ldms_prod
```

**SQL Server (Production):**
```
Server=prod-sql.yourdomain.com;Database=ldms_prod;User Id=appuser;Password=SECURE_PRODUCTION_PASSWORD;TrustServerCertificate=True
```

**How to configure (Choose one):**

Option 1: Environment Variables (Recommended for Production)
```bash
# Windows
set ConnectionStrings__Postgres=server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod

# Linux/Mac
export ConnectionStrings__Postgres="server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod"

# In .env file
ConnectionStrings__Postgres=server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod
```

Option 2: User Secrets (If not using docker/cloud)
```bash
dotnet user-secrets set "ConnectionStrings:Postgres" "server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod"
```

Option 3: Docker Environment Variable
```dockerfile
ENV ConnectionStrings__Postgres="server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=SECURE_PASSWORD;database=ldms_prod"
```

---

## 🗄️ Database Providers

### PostgreSQL (Recommended)

**Connection String Format:**
```
server=HOST;port=PORT;user id=USERNAME;password=PASSWORD;database=DATABASE_NAME
```

**Parameters:**
- `server` - Database server hostname/IP
- `port` - PostgreSQL port (default: 5432)
- `user id` - Database username
- `password` - Database password
- `database` - Database name

**Example:**
```
server=localhost;port=5432;user id=postgres;password=mypassword;database=ldms
```

**Additional Options:**
```
server=localhost;port=5432;user id=postgres;password=mypassword;database=ldms;SSL Mode=Require;
```

---

### SQL Server

**Connection String Format:**
```
Server=HOST;Database=DATABASE_NAME;User Id=USERNAME;Password=PASSWORD;TrustServerCertificate=True
```

**Parameters:**
- `Server` - Database server address
- `Database` - Database name
- `User Id` - Database username
- `Password` - Database password
- `TrustServerCertificate` - Trust self-signed certificates (dev/staging only)

**Example (Development):**
```
Server=(localdb)\MSSQLLocalDB;Database=ldms;TrustServerCertificate=True
```

**Example (Production):**
```
Server=prod-sql.yourdomain.com;Database=ldms_prod;User Id=appuser;Password=SECURE_PASSWORD;TrustServerCertificate=True
```

---

## 🔄 Switching Databases

### Development - PostgreSQL to SQL Server

1. Update `appsettings.Development.json`:
```json
{
  "DatabaseProvider": "SqlServer",
  "ConnectionStrings": {
    "SqlServer": "Server=(localdb)\\MSSQLLocalDB;Database=ldms_dev;TrustServerCertificate=True"
  }
}
```

2. Restart application
```bash
dotnet run
```

### Development - SQL Server to PostgreSQL

1. Update `appsettings.Development.json`:
```json
{
  "DatabaseProvider": "Postgres",
  "ConnectionStrings": {
    "Postgres": "server=localhost;port=5432;user id=postgres;password=password;database=ldms_dev"
  }
}
```

2. Restart application
```bash
dotnet run
```

---

## 🔐 Security Best Practices

### Development
✅ Store in `appsettings.Development.json` OR User Secrets
✅ Can use local database
✅ Non-sensitive passwords acceptable

### Staging
✅ Store in `appsettings.Staging.json` OR Environment Variables
✅ Use staging-specific database
✅ Use strong passwords
✅ Separate from production

### Production
✅ **ALWAYS** use Environment Variables or User Secrets
✅ **NEVER** in appsettings.Production.json
✅ Use strong, complex passwords
✅ Use dedicated database user (limited privileges)
✅ Rotate passwords regularly
✅ Use encrypted connections (SSL/TLS)

---

## 🔗 Connection String Format Reference

### PostgreSQL with SSL
```
server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=PASSWORD;database=ldms_prod;SSL Mode=Require;
```

### PostgreSQL with SSL Validation
```
server=prod-pg.yourdomain.com;port=5432;user id=appuser;password=PASSWORD;database=ldms_prod;SSL Mode=Require;Trust Server Certificate=true;
```

### SQL Server with Encryption
```
Server=prod-sql.yourdomain.com;Database=ldms_prod;User Id=appuser;Password=PASSWORD;Encrypt=true;TrustServerCertificate=False;
```

### Azure Database for PostgreSQL
```
server=servername.postgres.database.azure.com;port=5432;user id=username@servername;password=PASSWORD;database=database_name;SSL Mode=Require;
```

### Azure SQL Database
```
Server=tcp:servername.database.windows.net,1433;Initial Catalog=database_name;Persist Security Info=False;User ID=username;Password=PASSWORD;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;
```

### Amazon RDS PostgreSQL
```
server=instance-name.region.rds.amazonaws.com;port=5432;user id=username;password=PASSWORD;database=database_name;SSL Mode=Require;
```

### Amazon RDS SQL Server
```
Server=instance-name.region.rds.amazonaws.com,1433;Database=database_name;User Id=username;Password=PASSWORD;Encrypt=true;TrustServerCertificate=False;
```

---

## 📝 Setup Checklist

### Development Setup
- [ ] Choose database provider (Postgres or SQL Server)
- [ ] Ensure database server is running
- [ ] Configure connection string in `appsettings.Development.json`
- [ ] Test connection: `dotnet run`
- [ ] Verify no database errors in startup logs

### Staging Setup
- [ ] Choose database provider
- [ ] Ensure staging database server is running
- [ ] Configure connection string in `appsettings.Staging.json`
- [ ] Set `ASPNETCORE_ENVIRONMENT=Staging`
- [ ] Test connection: `dotnet run`
- [ ] Verify staging database is being used

### Production Setup
- [ ] Choose database provider
- [ ] Set `ASPNETCORE_ENVIRONMENT=Production` (permanently)
- [ ] Configure connection string via Environment Variables
- [ ] Test connection before deployment
- [ ] Use strong, unique password
- [ ] Enable SSL/TLS for database connection
- [ ] Document database details securely
- [ ] Set up automated backups
- [ ] Monitor database performance

---

## 🧪 Testing Database Connection

### From Command Line

**PostgreSQL:**
```bash
psql -h localhost -p 5432 -U postgres -d ldms_dev
```

**SQL Server:**
```bash
sqlcmd -S (localdb)\MSSQLLocalDB -d ldms -U SA
```

### From Application

Add this temporary endpoint to test:
```csharp
[HttpGet("test-connection")]
public async Task<IActionResult> TestConnection()
{
    try
    {
        using (var connection = new NpgsqlConnection(
            configuration.GetConnectionString("Postgres")))
        {
            await connection.OpenAsync();
            return Ok(new { status = "success", message = "Database connection successful" });
        }
    }
    catch (Exception ex)
    {
        return StatusCode(500, new { status = "error", message = ex.Message });
    }
}
```

---

## ❌ Common Connection Issues & Solutions

### "No database provider specified"
**Solution:** Set `DatabaseProvider` in appsettings to either `Postgres` or `SqlServer`

### "Connection timeout"
**Solution:** 
- Verify database server is running
- Check hostname/IP is correct
- Verify port is correct
- Check firewall rules

### "Authentication failed"
**Solution:**
- Verify username is correct
- Verify password is correct
- Check user has database access
- Verify database exists

### "SSL/TLS error"
**Solution:**
- Add `SSL Mode=Require` to PostgreSQL connection
- Add `Encrypt=true` to SQL Server connection
- Verify SSL certificate is valid
- For self-signed: use `TrustServerCertificate=True`

### "Connection string not found"
**Solution:**
- Verify connection string key matches configuration
- Check environment variable name uses double underscore: `ConnectionStrings__Postgres`
- Ensure `appsettings.{ENVIRONMENT}.json` exists

---

## 📚 Configuration Examples

### Example 1: Development with Local PostgreSQL
```json
// appsettings.Development.json
{
  "ConnectionStrings": {
    "Postgres": "server=localhost;port=5432;user id=postgres;password=dev123;database=ldms_dev"
  },
  "DatabaseProvider": "Postgres"
}
```

### Example 2: Staging with Remote PostgreSQL
```json
// appsettings.Staging.json
{
  "ConnectionStrings": {
    "Postgres": "server=staging-db.company.com;port=5432;user id=staging_user;password=staging_pass;database=ldms_staging"
  },
  "DatabaseProvider": "Postgres"
}
```

### Example 3: Production with Azure PostgreSQL
```bash
# Environment Variable
export ConnectionStrings__Postgres="server=myserver.postgres.database.azure.com;port=5432;user id=username@myserver;password=SECURE_PASS;database=ldms_prod;SSL Mode=Require;"
```

### Example 4: Production with RDS
```bash
# Environment Variable
export ConnectionStrings__Postgres="server=mydb.region.rds.amazonaws.com;port=5432;user id=admin;password=SECURE_PASS;database=ldms_prod;SSL Mode=Require;"
```

---

## 🔍 Verifying Configuration

Check which connection string is being used:

1. Look at startup logs
2. Add this endpoint temporarily:
```csharp
[HttpGet("config")]
public IActionResult GetConfig()
{
    return Ok(new {
        databaseProvider = configuration["DatabaseProvider"],
        connectionStringKey = configuration["DatabaseProvider"] == "SqlServer" ? "SqlServer" : "Postgres",
        environment = hostEnvironment.EnvironmentName
    });
}
```

3. Call: `GET https://localhost:5001/api/config`

---

## 📞 Support

For connection string help:
- PostgreSQL: See PostgreSQL documentation
- SQL Server: See SQL Server documentation  
- Cloud providers: See their specific guides

Always verify:
1. ✅ Database server is running
2. ✅ Credentials are correct
3. ✅ Database exists
4. ✅ Firewall allows connection
5. ✅ Connection string format is correct

---

**Version**: 1.0
**Updated**: January 2024
**Key Point**: Connection strings are environment-specific and must be configured separately for each environment.
