using Appointment_System.Application.DTOs.Appointment;
using Appointment_System.Application.DTOs.Patient;
using Appointment_System.Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Default: Admin only
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // GET: api/patient
        [HttpGet]
        public async Task<ActionResult<PagedResult<PatientDto>>> GetPatients([FromQuery] PatientQueryParams queryParams)
        {
            var result = await _patientService.GetPatientsAsync(queryParams);
            return Ok(result);
        }

        // GET: api/patient/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<PatientDto>> GetPatientById(string id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id);
            if (patient == null) return NotFound();
            return Ok(patient);
        }

        // DELETE: api/patient/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            await _patientService.DeletePatientAsync(id);
            return NoContent();
        }

        // GET: api/patient/{id}/appointments
        [HttpGet("{id}/appointments")]
        [Authorize(Roles = "Admin,Doctor")] // Allow Doctor & Admin
        public async Task<ActionResult<List<AppointmentDto>>> GetPatientAppointments(string id)
        {
            var appointments = await _patientService.GetPatientAppointmentsAsync(id);
            return Ok(appointments);
        }
    }
}
