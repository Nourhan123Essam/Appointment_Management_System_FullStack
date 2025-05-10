using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Infrastructure.Data;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Appointment_System.Infrastructure.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private readonly IRedisService _redisService;

        public AuthenticationRepository(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context,
            IRedisService redisService)
        {
            _userManager = userManager;
            ; _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
            _redisService = redisService;
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> Register(Patient appUser, string password)
        {
            //check of email correct
            var testUser = await _userManager.FindByEmailAsync(appUser.Email);
            if (testUser != null)
                return false;

            var user = new IdentityUser()
            {
                Email= appUser.Email,
                PhoneNumber = appUser.Phone,
                UserName = appUser.Email
            };
            var userResult = await _userManager.CreateAsync(user, password);
            if (!userResult.Succeeded)
                return false;

            // Assign "Patient" role by default
            await _userManager.AddToRoleAsync(user, "Patient");

            appUser.CreatedAt = DateTime.Now;
            appUser.UserId = user.Id;
            appUser.IsDeleted = false;
            await _context.Patients.AddAsync(appUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string?> GenerateTokenAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return null;

            return await GenerateToken(user);
        }

        public async Task<string?> GetUserIdByEmailAsync(string email)
        {
            var user = await GetUserByEmailAsync(email);                
            if (user == null)
                return null;

            return user.Id;
        }

        public async Task<bool> IsUserExist(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;
            return true;
        }

        public async Task<bool> UpdatePasswordAsync(string userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            var passwordHasher = new PasswordHasher<IdentityUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }


        public async Task<Domain.Responses.Response<LoginResult>> Login(string email, string password)
        {
            // 1. Try to find the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Domain.Responses.Response<LoginResult>.Fail("Invalid email");

            // 2. Check if the password is correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return Domain.Responses.Response<LoginResult>.Fail("Invalid password");

            // 3. Generate the JWT access token (short-lived)
            string accessToken = await GenerateToken(user);

            // 4. Generate a new secure refresh token (long-lived)
            string refreshToken = GenerateRefreshToken();

            // 5. Store the refresh token in Redis with expiration (key: "refresh_userId")
            await _redisService.SetRefreshTokenAsync(
                user.Id,
                refreshToken,
                TimeSpan.FromDays(7)
            );

            // 6. Prepare the response using the strongly typed LoginResult
            var loginResult = new LoginResult(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                ExpiresIn: 900 // 15 Minutes in seconds
            );

            // 7. Return a success response with the tokens
            return Domain.Responses.Response<LoginResult>.Success(loginResult, "Login successful");
        }


        // Generate Access Token
        private async Task<string> GenerateToken(IdentityUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Fetch all roles assigned to the user
            var userRoles = await _userManager.GetRolesAsync(user);

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15), // Short expiration
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // Generate Refresh Token
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

    }
}
