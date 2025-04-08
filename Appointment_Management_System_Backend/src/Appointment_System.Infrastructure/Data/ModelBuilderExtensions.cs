using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Data
{
    public static class ModelBuilderExtensions
    {
        public static void ApplyEntityRelationships(this ModelBuilder modelBuilder)
        {
            // Configure Appointment relationships
            modelBuilder.Entity<Appointment>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent accidental deletions

            modelBuilder.Entity<Appointment>()
                .HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent accidental deletions


            // Ensure Doctor Qualifications reference the correct User ID
            modelBuilder.Entity<DoctorQualification>()
                .HasOne<ApplicationUser>()  // Links to ApplicationUser
                .WithMany(d => d.Qualifications)
                .HasForeignKey(q => q.DoctorId) // Ensure FK is DoctorId, not ApplicationUserId
                .OnDelete(DeleteBehavior.Cascade);

            // Ensure Doctor Availabilities reference the correct User ID
            modelBuilder.Entity<DoctorAvailability>()
                .HasOne<ApplicationUser>()
                .WithMany(d => d.Availabilities)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
