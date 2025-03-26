using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorAvailabilityService _service;
        private readonly IDoctorQualificationService _qualification_service;


        public DoctorController(IDoctorAvailabilityService service, IDoctorQualificationService qualification_service)
        {
            _service = service;
            _qualification_service  = qualification_service;
        }

        // DoctorQualification CRUD operations
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<DoctorQualificationDto>>> GetQualificationByDoctorId(string doctorId)
        {
            var qualifications = await _qualification_service.GetByDoctorIdAsync(doctorId);
            return Ok(qualifications);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorQualificationDto>> GetQualificationById(int id)
        {
            var qualification = await _qualification_service.GetByIdAsync(id);
            if (qualification == null) return NotFound();
            return Ok(qualification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateQualification(string doctorId, CreateDoctorQualificationDto dto)
        {
            dto.DoctorId = doctorId;
            await _qualification_service.AddAsync(dto);
            return CreatedAtAction(nameof(GetQualificationByDoctorId), new { doctorId }, dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQualification(int id, UpdateDoctorQualificationDto dto)
        {
            await _qualification_service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQualification(int id)
        {
            await _qualification_service.DeleteAsync(id);
            return NoContent();
        }

        // DoctorAvailability CRUD operations

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorAvailabilityDto>> GetAvailabilityById(int id)
        {
            var availability = await _service.GetByIdAsync(id);
            if (availability == null) return NotFound();
            return Ok(availability);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorAvailabilityDto>>> GetAvailabilityByDoctorId(string doctorId)
        {
            return Ok(await _service.GetByDoctorIdAsync(doctorId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorAvailabilityDto dto)
        {
            await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetAvailabilityByDoctorId), new { doctorId = dto.DoctorId }, dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAvailability(int id, UpdateDoctorAvailabilityDto dto)
        {
            try
            {
                await _service.UpdateAsync(id, dto);
                return Ok(new { message = "Updated Successfully!"});
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
