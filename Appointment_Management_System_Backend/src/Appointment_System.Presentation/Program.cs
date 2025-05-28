using Appointment_System.Infrastructure;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Presentation.Middlewares;
using Serilog;
using Appointment_System.Application;
using System.Threading.RateLimiting;
using StackExchange.Redis;
using DotNetEnv;
using Appointment_System.Application.Localization;
using Appointment_System.Presentation.Localization;
using Appointment_System.Presentation.Extensions;

//"If you think good architecture is expensive, try bad architecture." - Brian Foote and Joseph Yoder


// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Example: log to the console
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day) // log to file, rolling logs daily
    .CreateLogger();


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Cleanly register infrastructure layer services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Cleanly register application layer services
builder.Services.AddApplicationServices();

// Add Serilog to the DI container and configure it as the logging provider
builder.Host.UseSerilog();

// Register built-in rate limiter service
builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 100,
            Window = TimeSpan.FromMinutes(1),
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = 0
        });
    });

    // return a proper 429 JSON response
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsync(
            "{\"error\": \"Rate limit exceeded. Please try again later.\"}",
            cancellationToken);
    };
});

// Register Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!));

// Session
// 1. Register Redis cache
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// 2. Register session service
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "AppointCare";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

// 3. Register the custom session middleware 
builder.Services.AddTransient<SessionValidationMiddleware>();

// Add CORS to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://appointment-frontend:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials() // Allow cookies (important!)
              .WithExposedHeaders("Authorization"); // Ensure Angular can access headers
    });
});

// Localizaion
builder.Services.AddLocalization();
builder.Services.AddSingleton<ILocalizationService, LocalizationService>();

// Register the middlewares with DI
builder.Services.AddTransient<ExceptionHandlingMiddleware>();


//*************************************************************************************

var app = builder.Build();

//*************************************************************************************

//Localization
var supportedCultures = new[] { "en-US", "ar-EG" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en-US")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);


// Activate the rate limiter middleware in the request pipeline
app.UseRateLimiter();

// Add the custom middlewares to the pipeline
app.UseTimeZoneMiddleware(); // using extension method for clean usage
app.UseLocalizationMiddleware(); // using extension method for clean usage
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use the CORS policy
app.UseCors("AllowSpecificOrigins");

// This loads variables from the .env file
Env.Load();

// Apply pending EF Core migrations and seed required roles/admin user
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DbSeeder.MigrateAndSeedAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error during DB setup: {ex.Message}");
    }
}


// Configure the HTTP request pipeline.
// Use swagger in any environment
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();     // Validates JWT token
app.UseAuthorization();      // Role policies, etc.

app.UseSession();
app.UseMiddleware<SessionValidationMiddleware>();  // Validates sessionId

app.MapControllers();

app.Run();
