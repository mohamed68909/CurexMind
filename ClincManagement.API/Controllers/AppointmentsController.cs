using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Appinments.Requests;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    [HttpGet]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> GetAllAppointments([FromQuery] int page = 1, [FromQuery] int pageSize = 10, CancellationToken cancel = default)
    {
        var result = await _appointmentService.GetAllAppointmentsAsync(page, pageSize, cancel);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpGet("{appointmentId:guid}")]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public async Task<IActionResult> GetAppointmentsById([FromRoute] Guid appointmentId, CancellationToken cancel)
    {
        
        var result = await _appointmentService.GetAppointmentsByIdAsync(appointmentId, cancel);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }

    [HttpPost]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> CreateAppointment([FromBody] CreateAppointmentDto request, CancellationToken cancel)
    {
        var result = await _appointmentService.CreateAppointmentAsync(request, cancel);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAppointmentsById), new { appointmentId = result.Value.AppointmentId }, result.Value)
            : result.ToProblem();
    }

    [HttpPost("patient/{patientId:guid}/book")]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public async Task<IActionResult> CreateAppointmentPatient([FromRoute] Guid patientId, [FromBody] BookAppointmentRequest request, CancellationToken cancel)
    {
       
        var result = await _appointmentService.CreateAppointmentPatientAsync(request, patientId, cancel);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetAppointmentsById), new { appointmentId = result.Value.AppointmentId }, result.Value)
            : result.ToProblem();
    }

    [HttpPut("{appointmentId:guid}")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> UpdateAppointment([FromRoute] Guid appointmentId, [FromBody] UpdateAppointmentDto request, CancellationToken cancel)
    {
        var result = await _appointmentService.UpdateAppointmentAsync(request, cancel);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpDelete("{appointmentId:guid}")]
    [Authorize(Roles = DefaultRoles.Admin.Name)]
    public async Task<IActionResult> DeleteAppointment([FromRoute] Guid appointmentId, CancellationToken cancel)
    {
        var result = await _appointmentService.DeleteAppointmentAsync(appointmentId, cancel);
        return result.IsSuccess ? NoContent() : result.ToProblem();
    }

    [HttpGet("patient/{patientId:guid}")]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public async Task<IActionResult> GetMyAppointments([FromRoute] Guid patientId, [FromQuery] AppointmentFilter filter, CancellationToken cancel)
    {
      
        var result = await _appointmentService.GetMyAppointmentsAsync(patientId, filter, cancel);
        return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
    }
}