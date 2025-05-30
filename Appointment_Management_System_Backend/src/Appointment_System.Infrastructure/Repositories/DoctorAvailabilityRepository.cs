﻿using Appointment_System.Application.Interfaces.Repositories;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories
{
    public class DoctorAvailabilityRepository : IDoctorAvailabilityRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorAvailabilityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Availability?> GetByIdAsync(int id)
        {
            return await _context.DoctorAvailabilities.FindAsync(id);
        }
        public async Task<IEnumerable<Availability>> GetByDoctorIdAsync(int doctorId)
        {
            return await _context.DoctorAvailabilities
                .Where(d => d.DoctorId == doctorId)
                .AsNoTracking() // No tracking for better performance
                .ToListAsync();
        }

        public async Task<int> AddAsync(Availability availability)
        {
            await _context.DoctorAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
            return availability.Id;
        }

        public async Task UpdateAsync(Availability availability)
        {
            _context.DoctorAvailabilities.Update(availability);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var availability = await _context.DoctorAvailabilities.FindAsync(id);
            if (availability != null)
            {
                _context.DoctorAvailabilities.Remove(availability);
                await _context.SaveChangesAsync();
            }
        }
    }

}
