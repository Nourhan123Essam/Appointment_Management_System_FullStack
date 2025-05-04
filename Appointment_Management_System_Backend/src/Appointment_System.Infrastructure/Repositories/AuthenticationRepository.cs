using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Infrastructure.Data;
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
        public AuthenticationRepository(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _context = context;
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


        public async Task<Response> Login(string email, string password)
        {

            //check of email correct
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new Response(false, "Invalid Email");

            //check if password correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (result == null)
            {
                return new Response(false, "Invalid Password");
            }

            //generate token and return it as the user vervified
            string token = await GenerateToken(user);
            return new Response(true, token);
        }

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
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
