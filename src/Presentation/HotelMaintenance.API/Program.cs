using HotelMaintenance.API.Hubs;
using HotelMaintenance.API.Middleware;
using HotelMaintenance.Application;
using HotelMaintenance.Identity;
using HotelMaintenance.Infrastructure;
using HotelMaintenance.Persistence;
using HotelMaintenance.Persistence.Context;
using HotelMaintenance.Persistence.Seeders;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();

// Add all layers
builder.Services.AddApplication();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddInfrastructure();
builder.Services.AddIdentityServices(builder.Configuration);

// Add SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<NotificationService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });

    options.AddPolicy("Production", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000" };
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for SignalR
    });
});

// Add API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Hotel Maintenance Management API",
        Version = "v1",
        Description = "REST API for Hotel Maintenance Management System",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@hotelmaintenance.com"
        }
    });

    // Add JWT authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Add response compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

// Add HTTP context accessor
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Run migrations and seed database on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        
        // Run migrations
        await context.Database.MigrateAsync();
        Log.Information("Database migrations applied successfully");

        // Seed initial data
        await DatabaseSeeder.SeedAsync(context);
        Log.Information("Database seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Fatal(ex, "An error occurred while migrating or seeding the database");
        throw;
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hotel Maintenance API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at the app's root
    });
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("Production");
    app.UseHsts();
}

// Add custom middleware
app.UseGlobalExceptionHandler();
app.UseRequestLogging();
app.UsePerformanceMonitoring();

app.UseHttpsRedirection();
app.UseResponseCompression();

// Authentication & Authorization (ORDER MATTERS!)
app.UseAuthentication();
app.UseAuthorization();

// Map endpoints
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Health check endpoint
app.MapHealthChecks("/health");

// Root endpoint
app.MapGet("/", () => Results.Redirect("/swagger"));

Log.Information("Application starting...");
app.Run();
Log.Information("Application shut down");
