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
    }
}
