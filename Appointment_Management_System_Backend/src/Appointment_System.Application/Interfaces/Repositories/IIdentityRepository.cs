namespace Appointment_System.Application.Interfaces.Repositories
{
    public interface IIdentityRepository
    {
        Task<string?> CreateUserAsync(string email, string password);
        Task<bool> AssignRoleAsync(string userId, string role);
        Task<bool> UserExistsAsync(string email);
        Task<bool> RoleExistsAsync(string role);
    }
}
