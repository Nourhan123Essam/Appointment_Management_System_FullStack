using Appointment_System.Infrastructure.Authorization.Handlers;
using Appointment_System.Infrastructure.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Appointment_System.Infrastructure.Authorization.Policies
{
    public static class AuthorizationPolicies
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            // Register custom requirement handler
            services.AddScoped<IAuthorizationHandler, EntityOwnershipHandler>();

            // Add custom policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("OwnDoctorProfile", policy =>
                    policy.Requirements.Add(new EntityOwnershipRequirement()));
            });

            return services;
        }
    }
}
