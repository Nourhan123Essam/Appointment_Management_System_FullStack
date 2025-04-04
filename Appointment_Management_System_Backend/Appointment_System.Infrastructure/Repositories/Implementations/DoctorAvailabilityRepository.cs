﻿using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories.Implementations
{
    public class DoctorAvailabilityRepository : IDoctorAvailabilityRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorAvailabilityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorAvailability?> GetByIdAsync(int id)
        {
            return await _context.DoctorAvailabilities.FindAsync(id);
        }
        public async Task<IEnumerable<DoctorAvailability>> GetByDoctorIdAsync(string doctorId)
        {
            return await _context.DoctorAvailabilities
                .Where(d => d.DoctorId == doctorId)
                .AsNoTracking() // No tracking for better performance
                .ToListAsync();
        }

        public async Task<int> AddAsync(DoctorAvailability availability)
        {
            await _context.DoctorAvailabilities.AddAsync(availability);
            await _context.SaveChangesAsync();
            return availability.Id;
        }

        public async Task UpdateAsync(DoctorAvailability availability)
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
