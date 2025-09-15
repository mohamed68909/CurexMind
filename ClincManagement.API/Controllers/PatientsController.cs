using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        readonly IPatientService _patientService;
        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }
        [HttpGet("{patientId}/appointments")]
        public async Task<IActionResult> GetAllAppointmentsByPatientIdAsync(Guid patientId)
        {
            var result = await _patientService.GetAllAppointmentsByPatientIdAsync(patientId);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Error);
            }
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePatientAsync([FromBody] PatientRequestDto request)
        {
            var result = await _patientService.CreatePatientAsync(request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }






    }
}
