using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Services.Implementaions;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Implementations;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Configure the database connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity with ApplicationUser and IdentityRole
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IAuthenticationRepository, AuthenticationRepository>();

// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins("http://localhost:4200") // Add your Angular app URL
              .AllowAnyMethod()
              .AllowAnyHeader()
        .AllowCredentials();
    });
});

var configuration = builder.Configuration;
builder.Services.Configure<RecaptchaSettings>(configuration.GetSection("Recaptcha"));


var app = builder.Build();


// Use the CORS policy
app.UseCors("AllowSpecificOrigins");


//  Ensure required roles and admin user exist
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
