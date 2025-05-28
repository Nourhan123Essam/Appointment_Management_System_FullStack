using Appointment_System.Application.Features.Office.Commands;
using Appointment_System.Application.Features.Office.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Appointment_System.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Admin only
    public class OfficeController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OfficeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("admin/all")]
        public async Task<IActionResult> GetAllOffices()
        {
            var result = await _mediator.Send(new GetAllOfficesQuery());

            if (!result.Succeeded)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpGet("{id}/translation")]
        public async Task<IActionResult> GetLocalizedOffice(int id, [FromQuery] string language)
        {
            var result = await _mediator.Send(new GetLocalizedOfficeQuery(id, language));

            if (!result.Succeeded)
                return NotFound(result.Message);

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOfficeById(int id)
        {
            var result = await _mediator.Send(new GetOfficeByIdQuery(id));

            if (!result.Succeeded)
                return NotFound(result.Message);

            return Ok(result);
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOfficeCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result); // returns { succeeded: true, message: "...", data: newOfficeId }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOffice(int id)
        {
            var result = await _mediator.Send(new DeleteOfficeCommand(id));
            if (!result.Succeeded)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPut("translation")]
        public async Task<IActionResult> UpdateTranslation([FromBody] UpdateOfficeTranslationCommand command)
        {
            var result = await _mediator.Send(command);
            if (!result.Succeeded)
                return NotFound(result);

            return Ok(result);
        }

    }
}
