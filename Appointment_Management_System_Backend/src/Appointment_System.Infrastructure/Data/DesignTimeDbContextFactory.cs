using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Appointment_System.Infrastructure.Data
{
    // This class helps EF Core create the ApplicationDbContext at design time (e.g., for migrations)
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Load configuration from appsettings.json manually because Program.cs is not executed
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
                .AddJsonFile("appsettings.json") // Load configuration from the main config file
                .Build();

            // Get the connection string from configuration
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Build the DbContextOptions using the connection string
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            // Return the configured context instance
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
