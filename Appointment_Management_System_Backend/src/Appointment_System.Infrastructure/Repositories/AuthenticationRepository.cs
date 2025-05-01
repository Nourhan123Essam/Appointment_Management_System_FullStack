using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticationRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }
        public async Task<bool> Register(User appUser, string password)
        {
            //check of email correct
            var testUser = await _userManager.FindByEmailAsync(appUser.Email);
            if (testUser != null)
                return false;

            var user = new ApplicationUser()
            {
                Email= appUser.Email,
                PhoneNumber = appUser.PhoneNumber,
                FullName = appUser.FullName,
                UserName = appUser.Email
            };
            var userResult = await _userManager.CreateAsync(user, password);
            if (!userResult.Succeeded)
                return false;

            // Assign "Patient" role by default
            await _userManager.AddToRoleAsync(user, "Patient");

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

        private async Task<string> GenerateToken(ApplicationUser user)
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
