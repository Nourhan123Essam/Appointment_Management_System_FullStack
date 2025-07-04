﻿using System;
using System.ComponentModel.DataAnnotations;
using Appointment_System.Domain.Common;

namespace Appointment_System.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        [Key]
        public int Id { get; set; } // Primary Key for Doctor

        public string UserId { get; set; }
        public string Phone {  get; set; }
        public string Email { get; set; }
        public string ImageUrl { get; set; } = null!;

        public decimal InitialFee { get; set; }
        public decimal FollowUpFee { get; set; }
        public int? FollowUpCount { get; set; } = 0;
        public int MaxFollowUps { get; set; }
        public int YearsOfExperience { get; set; }
        public int? RatingPoints { get; set; } = 0;
        public int? NumberOfRatings { get; set; } = 0;


        // Navigation
        public virtual ICollection<DoctorSpecialization> DoctorSpecializations { get; set; } = new List<DoctorSpecialization>();
        public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
        public virtual ICollection<Qualification> Qualifications { get; set; } = new List<Qualification>();
        public ICollection<DoctorTranslation> Translations { get; set; } = new List<DoctorTranslation>();

    }
}
