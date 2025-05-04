using Appointment_System.Infrastructure;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Presentation.Middlewares;
using Serilog;
using Appointment_System.Application;
using System.Threading.RateLimiting;

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


// Add CORS to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Your Angular app URL
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetIsOriginAllowed(origin => true) // Allow dynamic origins
              .WithExposedHeaders("Authorization"); // Ensure Angular can access headers
    });
});

// Register the middlewares with DI
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Activate the rate limiter middleware in the request pipeline
app.UseRateLimiter();

// Add the custom middlewares to the pipeline
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Use the CORS policy
app.UseCors("AllowSpecificOrigins");


//  Ensure required roles and admin user exist (Seeding)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var config = services.GetRequiredService<IConfiguration>();

    try
    {
        // Seed roles and admin user if they do not exist
        await DbSeeder.SeedRolesAndAdminAsync(services, config);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"? Error seeding database: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
