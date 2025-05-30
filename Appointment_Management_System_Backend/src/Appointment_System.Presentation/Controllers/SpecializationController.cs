using Appointment_System.Application.Features.Specializations.Commands;
using Appointment_System.Application.Features.Specializations.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Admin only
    public class SpecializationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SpecializationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        //== Select ==
        [HttpGet("count")]
        public async Task<IActionResult> GetCount()
        {
            var result = await _mediator.Send(new GetSpecializationCountQuery());
            return Ok(result);
        }

        [HttpGet("with-doctor-count")]
        public async Task<IActionResult> GetWithDoctorCount([FromQuery] string language)
        {
            var query = new GetSpecializationsWithDoctorCountQuery(language);
            var result = await _mediator.Send(query);

            return result.Succeeded
                ? Ok(result)
                : BadRequest(result);
        }

        [HttpGet("all-with-translations")]
        public async Task<IActionResult> GetAllWithTranslations()
        {
            var query = new GetAllSpecializationsWithTranslationsQuery();
            var result = await _mediator.Send(query);

            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpGet("{specializationId}/translation/{language}")]
        public async Task<IActionResult> GetTranslation(int specializationId, string language)
        {
            var query = new GetSpecializationTranslationQuery(specializationId, language);
            var result = await _mediator.Send(query);

            return result.Succeeded ? Ok(result) : NotFound(result);
        }



        //== Add ==
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSpecializationCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }


        //== Update ==
        [HttpPut("{id}/translations")]
        public async Task<IActionResult> UpdateTranslation(int id, [FromBody] UpdateSpecializationTranslationCommand command)
        {
            if (id != command.SpecializationId)
                return BadRequest("Mismatched ID.");

            var result = await _mediator.Send(command);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        //== Delete ==
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _mediator.Send(new DeleteSpecializationCommand(id));
            return result.Succeeded ? Ok(result) : NotFound(result);
        }

    }
}
