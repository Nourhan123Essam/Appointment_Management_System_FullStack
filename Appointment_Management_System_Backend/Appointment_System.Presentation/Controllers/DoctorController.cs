using Appointment_System.Application.DTOs.DoctorAvailability;
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

        public DoctorController(IDoctorAvailabilityService service)
        {
            _service = service;
        }


        // DoctorAvailability CRUD operations

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorAvailabilityDto>> GetById(int id)
        {
            var availability = await _service.GetByIdAsync(id);
            if (availability == null) return NotFound();
            return Ok(availability);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorAvailabilityDto>>> GetByDoctorId(string doctorId)
        {
            return Ok(await _service.GetByDoctorIdAsync(doctorId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateDoctorAvailabilityDto dto)
        {
            await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetByDoctorId), new { doctorId = dto.DoctorId }, dto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateDoctorAvailabilityDto dto)
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
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
