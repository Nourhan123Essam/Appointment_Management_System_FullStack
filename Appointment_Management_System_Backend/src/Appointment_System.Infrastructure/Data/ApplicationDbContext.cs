using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Dbsets:
        public DbSet<ApplicationUser> Users { get; set; }  
        public DbSet<IdentityRole> Roles { get; set; }    
        public DbSet<IdentityUserRole<string>> UserRoles {  get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorQualification> DoctorQualifications { get; set; }
        public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply custom relationships
            ModelBuilderExtensions.ApplyEntityRelationships(builder);

        }
    }
}
