using System.Text;
using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Services.Implementaions;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Implementations;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;


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

builder.Services.AddScoped<IDoctorAvailabilityRepository, DoctorAvailabilityRepository>();
builder.Services.AddScoped<IDoctorAvailabilityService, DoctorAvailabilityService>();

builder.Services.AddScoped<IDoctorQualificationRepository, DoctorQualificationRepository>();
builder.Services.AddScoped<IDoctorQualificationService, DoctorQualificationService>();

builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<IDoctorRepository, DoctorRepository>();




// Add JWT authentication to Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token."
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
            new string[] {}
        }
    });
});

//configure authentication and jwt 
var AccessAppSettings = builder.Configuration;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AccessAppSettings["Jwt:Issuer"],
        ValidAudience = AccessAppSettings["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(AccessAppSettings["Jwt:Key"])
        )
    };
});


var configuration = builder.Configuration;
builder.Services.Configure<RecaptchaSettings>(configuration.GetSection("Recaptcha"));

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
