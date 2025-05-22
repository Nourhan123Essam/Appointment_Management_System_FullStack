using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Appointment_System.Application.Interfaces;
using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Application.Interfaces.Services;
using Appointment_System.Application.Localization;
using Appointment_System.Domain.Entities;
using Appointment_System.Domain.Responses;
using Appointment_System.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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
        private readonly ISessionService _sessionService;
        private readonly ILocalizationService _localizer;
        private readonly IPatientRepository _patientRepository;

        public AuthenticationRepository(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            IRedisService redisService,
            ISessionService sessionService,
            ILocalizationService localizer,
            IPatientRepository patientRepository,
            ApplicationDbContext context
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _redisService = redisService;
            _sessionService = sessionService;
            _localizer = localizer;
            _patientRepository = patientRepository;
            _context = context;
        }

        public async Task<IdentityUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<Result<string>> Register(Patient appUser, string password)
        {
            var testUser = await _userManager.FindByEmailAsync(appUser.Email);
            if (testUser != null)
                return Result<string>.Fail(_localizer["EmailAlreadyExists"]);

            var identityUser = new IdentityUser()
            {
                Email = appUser.Email,
                PhoneNumber = appUser.Phone,
                UserName = appUser.Email
            };

            var userResult = await _userManager.CreateAsync(identityUser, password);
            if (!userResult.Succeeded)
                return Result<string>.Fail(_localizer["IdentityCreationFailed"]);

            var roleResult = await _userManager.AddToRoleAsync(identityUser, "Patient");
            if (!roleResult.Succeeded)
                return Result<string>.Fail(_localizer["RoleAssignmentFailed"]);

            try
            {
                appUser.CreatedAt = DateTime.Now;
                appUser.UserId = identityUser.Id;
                appUser.IsDeleted = false;

                await _patientRepository.AddAsync(appUser);

                return Result<string>.Success("", _localizer["UserRegisteredSuccessfully"]);
            }
            catch
            {
                return Result<string>.Fail(_localizer["DatabaseSaveFailed"]);
            }
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

            UserStore<IdentityUser> store = new UserStore<IdentityUser>(_context);
            var passwordHasher = new PasswordHasher<IdentityUser>();
            user.PasswordHash = passwordHasher.HashPassword(user, newPassword);
            await store.SetPasswordHashAsync(user, user.PasswordHash);
            await store.UpdateAsync(user);
            await _context.SaveChangesAsync();

            return true;
        }
       
        public async Task<Result<LoginResult>> Login(string email, string password)
        {
            // Step 1: Find the user by email
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<LoginResult>.Fail(_localizer["InvalidEmail"]);

            // Step 2: Check if the password is correct
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                return Result<LoginResult>.Fail(_localizer["InvalidPassword"]);

            // Step 3: Generate access and refresh tokens
            string accessToken = await GenerateToken(user);
            string refreshToken = GenerateRefreshToken();

            // Step 4: Save refresh token in Redis
            await _redisService.SetRefreshTokenAsync(user.Id, refreshToken, TimeSpan.FromDays(7));

            // Step 5: Create a session ID for this login
            var sessionId = await _sessionService.CreateSessionAsync(user.Id);

            // Step 6: Prepare the response
            var loginResult = new LoginResult(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                SessionId: sessionId,
                ExpiresIn: 900 // 15 minutes
            );

            return Result<LoginResult>.Success(loginResult, _localizer["LoginSuccessful"]);
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
