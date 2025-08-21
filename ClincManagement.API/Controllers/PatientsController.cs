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
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult Get()
        {
            var id = HttpContext.GetRouteValue("id") as string;
            if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out Guid patientId))
            {
                return BadRequest("Invalid patient ID.");
            }
            var patient = _patientService.GetPatientByIdAsync(patientId);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }
        [HttpGet]
        public async Task<IActionResult> GetPatients([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var patients = await _patientService.GetPatientsAsync(search, page, pageSize);
            return Ok(patients);
        }
        [HttpPost]
        public async Task<IActionResult> CreatePatient([FromBody] PatientRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid patient data.");
            }
            var createdPatient = await _patientService.CreatePatientAsync(request);
            return CreatedAtAction(nameof(Get), new { id = createdPatient.PatientId}, createdPatient);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdatePatient(Guid id, [FromBody] PatientRequestDto request)
        {
            if (request == null)
            {
                return BadRequest("Invalid patient data.");
            }
            var updatedPatient = await _patientService.UpdatePatientAsync(id, request);
            if (updatedPatient == null)
            {
                return NotFound();
            }
            return Ok(updatedPatient);
        }
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeletePatient(Guid id)
        {
            var result = await _patientService.DeletePatientAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }


        }
}
