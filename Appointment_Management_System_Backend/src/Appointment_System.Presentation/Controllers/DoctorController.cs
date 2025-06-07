using System.Diagnostics;
using System.IO;
using Appointment_System.Application.DTOs.Doctor;
using Appointment_System.Application.DTOs.DoctorAvailability;
using Appointment_System.Application.DTOs.DoctorQualification;
using Appointment_System.Application.Features.Doctor.Commands;
using Appointment_System.Application.Features.Doctor.Queries;
using Appointment_System.Application.Features.DoctorAvailabilities.Commands;
using Appointment_System.Application.Features.DoctorAvailabilities.Queries;
using Appointment_System.Application.Features.DoctorQualifications.Commands;
using Appointment_System.Application.Features.DoctorQualifications.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [EnableCors("AllowSpecificOrigins")]  // Apply CORS directly on controller
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public DoctorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //////////////////////////////// 1 ///////////////////////////////////////
        // Doctor CRUD operations
        #region Doctor
        [Authorize(Policy = "OwnDoctorProfile")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var result = await _mediator.Send(new GetDoctorByIdQuery(id));
            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("public")]
        public async Task<IActionResult> GetDoctors([FromBody] DoctorFilterOptions filterOptions)
        {
            var stopwatch = Stopwatch.StartNew();

            var query = new GetPublicDoctorsQuery(filterOptions);
            var result = await _mediator.Send(query);

            stopwatch.Stop();
            Console.WriteLine($"[GetDoctors] Execution time: {stopwatch.ElapsedMilliseconds} ms");

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }


        [HttpGet("top5")]
        public async Task<IActionResult> GetTop5Doctors([FromQuery] string language)
        {
            var result = await _mediator.Send(new GetTop5DoctorsQuery(language));
            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetDoctorsForAdmin([FromBody] DoctorFilterOptions filterOptions)
        {
            var query = new GetAdminDoctorsQuery(filterOptions);
            var result = await _mediator.Send(query);
            return result.Succeeded
                ? Ok(result.Data)
                : BadRequest(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateDoctor([FromBody] CreateDoctorDto doctorDto)
        {
            var result = await _mediator.Send(new CreateDoctorCommand(doctorDto, doctorDto.Password));

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [Authorize(Policy = "OwnDoctorProfile")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor([FromBody] UpdateDoctorDto dto)
        {
            var result = await _mediator.Send(new UpdateDoctorCommand(dto));

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);

        }

        [Authorize(Policy = "OwnDoctorProfile")]
        [HttpPut("translation/{id}")]
        public async Task<IActionResult> UpdateTranslation([FromBody] DoctorTranslationDto dto)
        {
            var command = new UpdateDoctorTranslationCommand(dto);

            var result = await _mediator.Send(command);

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result.Message);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{doctorId}")]
        public async Task<IActionResult> DeleteDoctor(int doctorId)
        {
            var result = await _mediator.Send(new DeleteDoctorCommand(doctorId));

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }

        #endregion

        //////////////////////////////// 2 ///////////////////////////////////////
        // DoctorQualification CRUD operations
        #region Qualifications
        [Authorize(Roles = "Admin")]
        [HttpGet("GetQualificationByDoctorId")]
        public async Task<ActionResult<List<DoctorQualificationDto>>> GetQualificationByDoctorId(int doctorId)
        {
            var result = await _mediator.Send(new GetDoctorQualificationsByDoctorIdQuery(doctorId));
            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetQualificationById/{id}")]
        public async Task<ActionResult<DoctorQualificationDto>> GetQualificationById(int id)
        {
            var result = await _mediator.Send(new GetDoctorQualificationByIdQuery(id));
            if (result == null)
                return NotFound("Qualification not found");

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateQualification")]
        public async Task<IActionResult> CreateQualification(CreateDoctorQualificationDto dto)
        {
            var result = await _mediator.Send(new CreateDoctorQualificationCommand(dto));
            return CreatedAtAction(nameof(GetQualificationByDoctorId), new { doctorId = result.DoctorId }, result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateQualification/{id}")]
        public async Task<IActionResult> UpdateQualification(int id, UpdateDoctorQualificationDto dto)
        {
            await _mediator.Send(new UpdateDoctorQualificationCommand(id, dto));
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteQualification/{id}")]
        public async Task<IActionResult> DeleteQualification(int id)
        {
            await _mediator.Send(new DeleteDoctorQualificationCommand(id));
            return NoContent(); // 204 No Content as confirmation of deletion
        }
        #endregion
        //////////////////////////////// 3 ///////////////////////////////////////
        // DoctorAvailability CRUD operations
        #region Availabilities
        [Authorize(Roles = "Admin")]
        [HttpGet("GetAvailabilityById/{id}")]
        public async Task<ActionResult<DoctorAvailabilityDto>> GetAvailabilityById(int id)
        {
            var availability = await _mediator.Send(new GetDoctorAvailabilityByIdQuery(id));

            if (availability == null)
                return NotFound("Doctor availability not found.");

            return Ok(availability);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAvailabilityByDoctorId/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorAvailabilityDto>>> GetAvailabilityByDoctorId(int doctorId)
        {
            var availabilities = await _mediator.Send(new GetDoctorAvailabilitiesByDoctorIdQuery(doctorId));
            return Ok(availabilities);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("CreateAvailability")]
        public async Task<IActionResult> CreateAvailability(CreateDoctorAvailabilityDto dto)
        {
            var availabilityDto = await _mediator.Send(new CreateDoctorAvailabilityCommand(dto));
            return CreatedAtAction(nameof(GetAvailabilityByDoctorId), new { doctorId = availabilityDto.DoctorId }, availabilityDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateAvailability/{id}")]
        public async Task<IActionResult> UpdateAvailability(int id, UpdateDoctorAvailabilityDto dto)
        {
            var result = await _mediator.Send(new UpdateDoctorAvailabilityCommand(id, dto));

            if (result)
            {
                return Ok(new { message = "Updated Successfully!" });
            }

            return NotFound("Doctor availability not found.");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteAvailability/{id}")]
        public async Task<IActionResult> DeleteAvailability(int id)
        {
            var result = await _mediator.Send(new DeleteDoctorAvailabilityCommand(id));
            if (result)
            {
                return Ok(new { message = "Deleted Successfully!" });
            }
            return NotFound("Doctor availability not found.");
        }
        #endregion
    }
}
