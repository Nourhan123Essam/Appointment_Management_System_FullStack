using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Appointment_System.Domain.Entities;
using Appointment_System.Infrastructure.Data;
using Appointment_System.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Appointment_System.Infrastructure.Repositories.Implementations
{
    public class DoctorQualificationRepository : IDoctorQualificationRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorQualificationRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DoctorQualification?> GetByIdAsync(int id)
        {
            return await _context.DoctorQualifications.FindAsync(id);
        }

        public async Task<IEnumerable<DoctorQualification>> GetByDoctorIdAsync(string doctorId)
        {
            return await _context.DoctorQualifications
                .Where(q => q.DoctorId == doctorId)
                .ToListAsync();
        }

        public async Task<int> AddAsync(DoctorQualification qualification)
        {
            await _context.DoctorQualifications.AddAsync(qualification);
            await _context.SaveChangesAsync();
            return qualification.Id;
        }

        public async Task UpdateAsync(DoctorQualification qualification)
        {
            _context.DoctorQualifications.Update(qualification);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var qualification = await _context.DoctorQualifications.FindAsync(id);
            if (qualification != null)
            {
                _context.DoctorQualifications.Remove(qualification);
                await _context.SaveChangesAsync();
            }
        }
    }

}
