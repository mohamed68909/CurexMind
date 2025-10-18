using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Contracts.Appinments.Respones;
using ClincManagement.API.Contracts.Appointments.Responses;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PagedAppointmentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAppointments(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancel = default)
        {
            var result = await _appointmentService.GetAllAppointmentsAsync(page, pageSize, cancel);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("patient/{patientId}")]
        [Authorize]
        [ProducesResponseType(typeof(AppointmentDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAppointmentsByPatientId(
            [FromRoute] Guid patientId,
            CancellationToken cancel)
        {
            var result = await _appointmentService.GetAppointmentsByPatientIdAsync(patientId, cancel);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAppointment(
            [FromBody] CreateAppointmentDto request,
            CancellationToken cancel)
        {
            var result = await _appointmentService.CreateAppointmentAsync(request, cancel);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetAppointmentsByPatientId), new { patientId = result.Value.AppointmentId }, result.Value)
                : result.ToProblem();
        }

        [HttpPost("patient/{patientId}/book")]
        [Authorize]
        [ProducesResponseType(typeof(ResponserAppointmentDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateAppointmentPatient(
            [FromRoute] Guid patientId,
            [FromBody] BookAppointmentRequest request,
            CancellationToken cancel)
        {
            var result = await _appointmentService.CreateAppointmentPatientAsync(request, patientId, cancel);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetAppointmentsByPatientId), new { patientId = patientId }, result.Value)
                : result.ToProblem();
        }

        [HttpPut]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateAppointment(
            [FromBody] UpdateAppointmentDto request,
            CancellationToken cancel)
        {
            var result = await _appointmentService.UpdateAppointmentAsync(request, cancel);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("{appointmentId}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteAppointment(
            [FromRoute] Guid appointmentId,
            CancellationToken cancel)
        {
            var result = await _appointmentService.DeleteAppointmentAsync(appointmentId, cancel);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}



