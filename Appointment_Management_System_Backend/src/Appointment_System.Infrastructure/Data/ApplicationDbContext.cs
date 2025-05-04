using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // Dbsets:
        public DbSet<IdentityUser> Users { get; set; }  
        public DbSet<IdentityRole> Roles { get; set; }    
        public DbSet<IdentityUserRole<string>> UserRoles {  get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Qualification> DoctorQualifications { get; set; }
        public DbSet<Availability> DoctorAvailabilities { get; set; }
        public DbSet<AvailabilityException> AvailabilityExceptions { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<DoctorSpecialization> DoctorSpecializations { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Medicine> Medicines { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Office> Offices { get; set; }                                                          
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }
        public DbSet<Specialization> Specializations { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply custom relationships
            //ModelBuilderExtensions.ApplyEntityRelationships(builder);

            // Apply entity configurations
            builder.ApplyConfiguration(new DoctorSpecializationConfiguration());
            builder.ApplyConfiguration(new AppointmentConfiguration());
            builder.ApplyConfiguration(new AvailabilityConfiguration());
            builder.ApplyConfiguration(new AvailabilityExceptionConfiguration());
            builder.ApplyConfiguration(new ChatConfiguration());
            builder.ApplyConfiguration(new DoctorConfiguration());
            builder.ApplyConfiguration(new FeedbackConfiguration());
            builder.ApplyConfiguration(new MedicineConfiguration());
            builder.ApplyConfiguration(new MessageConfiguration());
            builder.ApplyConfiguration(new OfficeConfiguration());
            builder.ApplyConfiguration(new PatientConfiguration());
            builder.ApplyConfiguration(new PrescriptionConfiguration());
            builder.ApplyConfiguration(new QualificationConfiguration());
            builder.ApplyConfiguration(new SpecializationConfiguration());
        }
    }
}
