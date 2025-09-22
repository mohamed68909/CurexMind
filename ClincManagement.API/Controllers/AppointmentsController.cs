using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
 
        public class AppointmentsController : ControllerBase
        {
            private readonly IAppointmentService _appointmentService;

            public AppointmentsController(IAppointmentService appointmentService)
            {
                _appointmentService = appointmentService;
            }

           
            [HttpGet]
            [ProducesResponseType(typeof(PagedAppointmentResponse), StatusCodes.Status200OK)]
            [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
            public async Task<IActionResult> GetAllAppointmentsAsync([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
            {
                var result = await _appointmentService.GetAllAppointmentsAsync(page, pageSize, cancellationToken);

                return result.IsSuccess
                    ? Ok(result.Value)
                    : result.ToProblem();
            }

            // ✅ Create
            [HttpPost]
            [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
            [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> CreateAppointmentAsync([FromBody] CreateAppointmentDto request, CancellationToken cancellationToken)
            {
                var result = await _appointmentService.CreateAppointmentAsync(request, cancellationToken);

                return result.IsSuccess
                    ? Ok(result.Value)
                    : result.ToProblem();
            }

           
            [HttpPut("{id:guid}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
            [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> UpdateAppointmentAsync(Guid id, [FromBody] UpdateAppointmentDto request, CancellationToken cancellationToken)
            {
                if (id != request.AppointmentId)
                    return BadRequest("Appointment ID mismatch");

                var result = await _appointmentService.UpdateAppointmentAsync(request, cancellationToken);

                return result.IsSuccess
                    ? NoContent()
                    : result.ToProblem();
            }

         
            [HttpDelete("{id:guid}")]
            [ProducesResponseType(StatusCodes.Status204NoContent)]
            [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
            [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
            public async Task<IActionResult> DeleteAppointmentAsync(Guid id, CancellationToken cancellationToken)
            {
                var result = await _appointmentService.DeleteAppointmentAsync(id, cancellationToken);

                return result.IsSuccess
                    ? NoContent()
                    : result.ToProblem();
            }
        }
    }


