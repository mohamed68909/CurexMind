using ClincManagement.API.Abstractions;
using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Operation.Response;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Contracts.Patient.Responses;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientResponseDto = ClincManagement.API.Contracts.Patient.Respones.PatientResponseDto;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

       
        [HttpGet]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PagedPatientResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientsAsync(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.GetPatientsAsync(search, page, pageSize);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpGet("{id:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            
            var result = await _patientService.GetPatientByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PatientCreateResponseDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePatientAsync([FromForm] PatientRequestDto request, CancellationToken cancellationToken)
        {
            var createdByUserId = User.GetUserId(); 
            var result = await _patientService.CreateAsync(createdByUserId, request.ProfileImageUrl, request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPatientById), new { id = result.Value.PatientId }, result.Value)
                : result.ToProblem();
        }

       
        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePatientAsync(Guid id, [FromForm] PatientRequestDto request)
        {
           
            var result = await _patientService.UpdatePatientAsync(id, request.ProfileImageUrl, request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeletePatientAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _patientService.Delete(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        
        [HttpGet("{patientId:guid}/appointments")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        public async Task<IActionResult> GetAllAppointmentsByPatientIdAsync(Guid patientId)
        {
            
            var result = await _patientService.GetAllAppointmentsByPatientIdAsync(patientId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpGet("{patientId:guid}/invoices")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        public async Task<IActionResult> GetAllInvoicesByPatientId(Guid patientId)
        {
          
            var result = await _patientService.GetAllInvoicesByPatientIdAsync(patientId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpGet("{patientId:guid}/stays")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        public async Task<IActionResult> GetAllStaysByPatientIdAsync(Guid patientId)
        {
            
            var result = await _patientService.GetAllStaysByPatientIdAsync(patientId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}