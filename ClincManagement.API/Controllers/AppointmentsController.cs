using ClincManagement.API.Contracts.Appinments.Requests;
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
        [HttpGet("{appointmentId}")]
        public IActionResult Get()
        {
            var appointmentId = Guid.NewGuid(); // Replace with actual appointment ID
            var appointment = _appointmentService.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                return NotFound();
            }
            return Ok(appointment);
        }
        [HttpGet]
        public IActionResult GetAll()
        {
            var appointments = _appointmentService.GetAllAppointmentsAsync();
            if (appointments == null || appointments.IsCanceled)
            {
                return NotFound();
            }
            return Ok(appointments);
        }
        [HttpPost]
        public IActionResult Create([FromBody] CreateRequestAppointment request)
        {
            if (request == null)
            {
                return BadRequest("Invalid appointment data.");
            }
            var createdAppointment = _appointmentService.CreateAppointmentAsync(request);
            if (createdAppointment == null)
            {
                return BadRequest("Failed to create appointment.");
            }
            return CreatedAtAction(nameof(Get), new { appointmentId = createdAppointment.Id }, createdAppointment);
        }
        [HttpPut("{appointmentId}")]
        public IActionResult Update(Guid appointmentId, [FromBody] CreateRequestAppointment request)
        {
            if (request == null || appointmentId == Guid.Empty)
            {
                return BadRequest("Invalid appointment data or ID.");
            }
            var updatedAppointment = _appointmentService.UpdateAppointmentAsync(appointmentId, request);
            if (updatedAppointment == null)
            {
                return NotFound();
            }
            return Ok(updatedAppointment);
        }

        [HttpDelete("{appointmentId}")]
        public IActionResult Delete(Guid appointmentId)
        {

            if (appointmentId == Guid.Empty)
            {
                return BadRequest("Invalid appointment ID.");
            }
            var isDeleted = _appointmentService.DeleteAppointmentAsync(appointmentId);
            
            return NoContent();
        }

    }
}
