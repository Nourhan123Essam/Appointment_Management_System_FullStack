using Appointment_System.Application.Services.Implementaions;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Infrastructure;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Presentation.Middlewares;
using Serilog;


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

// Application layer services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();
builder.Services.AddScoped<IDoctorQualificationService, DoctorQualificationService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IPatientService, PatientService>();

// Cleanly register infrastructure layer services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add Serilog to the DI container and configure it as the logging provider
builder.Host.UseSerilog();


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
//builder.Services.AddScoped<RequestLoggingMiddleware>();


var app = builder.Build();

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
