using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Features.Patient.Commands;
using Appointment_System.Application.Features.Patient.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Default: Admin only
    public class PatientController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatientController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/patient
        [HttpGet]
        public async Task<ActionResult<PagedResult<PatientDto>>> GetPatients([FromQuery] PatientQueryParams queryParams)
        {
            var result = await _mediator.Send(new GetPatientsQuery(queryParams));
            return Ok(result);
        }

        // GET: api/patient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(int id)
        {
            var result = await _mediator.Send(new GetPatientByIdQuery(id));
            return result == null ? NotFound() : Ok(result);
        }

        // DELETE: api/patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var success = await _mediator.Send(new DeletePatientCommand(id));
            return success ? NoContent() : NotFound();
        }

        // GET: api/patient/{id}/appointments
        [HttpGet("{id}/appointments")]
        [Authorize(Roles = "Admin,Doctor")] // Allow Doctor & Admin
        public async Task<ActionResult<List<AppointmentDto>>> GetPatientAppointments(int id)
        {
            var appointments = await _mediator.Send(new GetPatientAppointmentsQuery(id));
            return Ok(appointments);
        }
    }
}
