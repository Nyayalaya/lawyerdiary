using CourtApp.Api.Extensions;
using CourtApp.Application.DTOs.Settings;
using CourtApp.Application.Extensions;
using CourtApp.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

Console.WriteLine($"CourtApp.Api starting in {environment} environment");

// API Layer Services
builder.Services.AddApiServices();

// Application Layer
builder.Services.AddApplicationLayer();

// Infrastructure Layer
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddIdentityInfrastructure(builder.Configuration);
builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWTSettings"));

var app = builder.Build();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");

// Development tools
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CourtApp API v1");
        c.RoutePrefix = "swagger";
    });
}

// API Middleware
app.UseApiMiddleware();

app.MapControllers();

Console.WriteLine("CourtApp.Api started successfully");

app.Run();
