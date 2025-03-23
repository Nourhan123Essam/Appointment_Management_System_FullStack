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


            // DoctorQualification Relationship
            modelBuilder.Entity<DoctorQualification>()
                .HasOne<ApplicationUser>()
                .WithMany() // No navigation in ApplicationUser
                .HasForeignKey(q => q.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // DoctorAvailability Relationship
            modelBuilder.Entity<DoctorAvailability>()
                .HasOne<ApplicationUser>()
                .WithMany() // No navigation in ApplicationUser
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);
        }


    }
}
