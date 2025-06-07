using Microsoft.AspNetCore.Authorization;

namespace Appointment_System.Infrastructure.Authorization.Requirements
{
    // Represents a rule to check if a user owns a specific resource (like a doctor profile)
    public class EntityOwnershipRequirement : IAuthorizationRequirement
    {
        public EntityOwnershipRequirement() { }
    }
}
