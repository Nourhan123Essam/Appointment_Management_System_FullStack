using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Services.Implementaions;
using Appointment_System.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [EnableCors("AllowSpecificOrigins")]  // Apply CORS directly on controller
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IDoctorAvailabilityService _service;
        private readonly IDoctorQualificationService _qualification_service;
        private readonly IDoctorService _doctorService;

        public DoctorController(IDoctorAvailabilityService service, IDoctorQualificationService qualification_service, IDoctorService doctorService)
        {
            _service = service;
            _qualification_service  = qualification_service;
            _doctorService = doctorService;
        }

        //////////////////////////////// 1 ///////////////////////////////////////
        // Doctor CRUD operations

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var doctor = await _doctorService.GetDoctorByIdAsync(id);
            if (doctor == null)
                return NotFound($"Doctor with ID {id} not found.");

            return Ok(doctor);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody]DoctorCreateDto doctorDto)
        {
            var doctor = await _doctorService.CreateDoctorAsync(doctorDto);
            if (doctor == null)
                return BadRequest("Failed to create doctor.");

            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }

        [HttpGet("doctorsBasicData")]
        public async Task<IActionResult> GetAllDoctorsBasicData()
        {
            var doctors = await _doctorService.GetAllDoctorsBasicDataAsync();
            return Ok(doctors);
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetAllDoctors()
        {
            var doctors = await _doctorService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{doctorId}")]
        public async Task<IActionResult> UpdateDoctor(string doctorId, [FromBody] DoctorUpdateDto dto)
        {
            var updated = await _doctorService.UpdateDoctorAsync(doctorId, dto);

            if (!updated)
                return NotFound("Doctor not found.");

            return Ok(new { message = "Doctor updated successfully." });

        }

        [HttpDelete("{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(string doctorId)
        {
            var deleted = await _doctorService.DeleteDoctorAsync(doctorId);

            if (!deleted)
                return NotFound("Doctor not found.");

            return Ok(new { message = "Doctor deleted successfully." });

        }




        //////////////////////////////// 2 ///////////////////////////////////////
        // DoctorQualification CRUD operations
        [Authorize(Roles = "Admin")]
        [HttpGet("GetQualificationByDoctorId")]
        public async Task<ActionResult<List<DoctorQualificationDto>>> GetQualificationByDoctorId(string doctorId)
        {
            var qualifications = await _qualification_service.GetByDoctorIdAsync(doctorId);
            return Ok(qualifications);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetQualificationById/{id}")]
        public async Task<ActionResult<DoctorQualificationDto>> GetQualificationById(int id)
        {
            var qualification = await _qualification_service.GetByIdAsync(id);
            return Ok(qualification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateQualification")]
        public async Task<IActionResult> CreateQualification(CreateDoctorQualificationDto dto)
        {
            var newQualification = await _qualification_service.AddAsync(dto);
            return CreatedAtAction(nameof(GetQualificationByDoctorId), new { doctorId = dto.DoctorId }, newQualification);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateQualification/{id}")]
        public async Task<IActionResult> UpdateQualification(int id, UpdateDoctorQualificationDto dto)
        {
            await _qualification_service.UpdateAsync(id, dto);
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteQualification/{id}")]
        public async Task<IActionResult> DeleteQualification(int id)
        {
            await _qualification_service.DeleteAsync(id);
            return NoContent();
        }

        //////////////////////////////// 3 ///////////////////////////////////////
        // DoctorAvailability CRUD operations

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAvailabilityById/{id}")]
        public async Task<ActionResult<DoctorAvailabilityDto>> GetAvailabilityById(int id)
        {
            var availability = await _service.GetByIdAsync(id);
            if (availability == null) return NotFound();
            return Ok(availability);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAvailabilityByDoctorId/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorAvailabilityDto>>> GetAvailabilityByDoctorId(string doctorId)
        {
            return Ok(await _service.GetByDoctorIdAsync(doctorId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateAvailability")]
        public async Task<IActionResult> CreateAvailability(CreateDoctorAvailabilityDto dto)
        {
            var newAvailability = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetAvailabilityByDoctorId), new { doctorId = dto.DoctorId }, newAvailability);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateAvailability/{id}")]
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
        [HttpDelete("DeleteAvailability/{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
