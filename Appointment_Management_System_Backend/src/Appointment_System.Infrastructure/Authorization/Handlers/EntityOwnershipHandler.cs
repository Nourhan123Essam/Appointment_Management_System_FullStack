using System.Security.Claims;
using Appointment_System.Infrastructure.Authorization.Requirements;
using Appointment_System.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Authorization.Handlers
{
    // Checks if the user is the owner of the requested entity or is an Admin
    public class EntityOwnershipHandler : AuthorizationHandler<EntityOwnershipRequirement>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        public EntityOwnershipHandler(IHttpContextAccessor accessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = accessor;
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, EntityOwnershipRequirement requirement)
        {
            var user = context.User;
            var httpContext = _httpContextAccessor.HttpContext;

            // Admins always pass
            if (user.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return;
            }

            // Ensure user is Doctor
            if (!user.IsInRole("Doctor"))
                return;

            // Get user ID from JWT (sub claim)
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value; // or "sub"

            if (string.IsNullOrEmpty(userId))
                return;

            // Get doctorId from route
            var routeDoctorId = httpContext?.Request.RouteValues["id"]?.ToString();
            if (!int.TryParse(routeDoctorId, out var doctorId))
                return;

            // Fetch doctor record with that UserId
            var doctor = await _dbContext.Doctors
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.UserId == userId);

            if (doctor != null && doctor.Id == doctorId)
            {
                context.Succeed(requirement);
            }
        }
    }
}
