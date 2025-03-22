using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Application.DTOs.Authentication;
using Appointment_System.Application.Services.Interfaces;
using Appointment_System.Domain.Responses;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Appointment_System.Application.Services.Implementaions
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAuthenticationRepository authenticationRepository;

        public AuthenticationService(IAuthenticationRepository _authenticationRepository)
        {
            authenticationRepository = _authenticationRepository;
        }
        public async Task<Response> RegisterAsync(RegisterDTO request)
        {
            // Check if the user already exists
            var getUser = await authenticationRepository.GetUserByEmailAsync(request.Email);
            if (getUser is not null)
                return new Response(false, $"This email is already registered");

            // Create a new user
            var newUser = new ApplicationUser()
            {
                Email = request.Email,
                UserName = request.Email,
                PhoneNumber = request.TelephoneNumber,
                FullName = request.FullName
            };

            var result = await authenticationRepository.Register(newUser, request.Password);

            if(!result) return new Response(false, "Invalid data provided");
            return new Response(true, "User registered successfully");

        }
        public async Task<Response> LoginAsync(LoginDTO request)
        {
            // Check if the user already exists
            var getUser = await authenticationRepository.GetUserByEmailAsync(request.Email);
            if (getUser is null)
                return new Response(false, "Invalid credentials");

            return await authenticationRepository.Login(getUser, request.Password);

        }

    }
}
