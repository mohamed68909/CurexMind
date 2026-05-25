using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Abstractions.Consts;
using ClinicManagement.API.Contracts.Operation.Response;
using ClinicManagement.API.Contracts.Patient.Requests;
using ClinicManagement.API.Contracts.Patient.Respones;
using ClinicManagement.API.Contracts.Patient.Responses;
using ClinicManagement.API.Services.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientResponseDto = ClinicManagement.API.Contracts.Patient.Respones.PatientResponseDto;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly IValidator<PatientRequestDto> _validator;

        public PatientsController(
            IPatientService patientService,
            IValidator<PatientRequestDto> validator)
        {
            _patientService = patientService;
            _validator = validator;
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
            if (!User.HasAccessToPatient(id))
                return Forbid();

            var result = await _patientService.GetPatientByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PatientCreateResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePatientAsync([FromForm] PatientRequestDto request, CancellationToken cancellationToken)
        {
            // Validate request using FluentValidation before calling the service
            var validation = await _validator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            var createdByUserId = User.GetUserId();
            var result = await _patientService.CreateAsync(createdByUserId, request.ProfileImage, request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPatientById), new { id = result.Value.PatientId }, result.Value)
                : result.ToProblem();
        }

       
        [HttpPut("{id:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatientAsync(Guid id, [FromForm] PatientRequestDto request)
        {
            if (!User.HasAccessToPatient(id))
                return Forbid();

            // Validate request using FluentValidation before calling the service
            var validation = await _validator.ValidateAsync(request);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            var result = await _patientService.UpdatePatientAsync(id, request.ProfileImage, request);
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
            if (!User.HasAccessToPatient(patientId))
                return Forbid();

            var result = await _patientService.GetAllAppointmentsByPatientIdAsync(patientId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpGet("{patientId:guid}/invoices")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        public async Task<IActionResult> GetAllInvoicesByPatientId(Guid patientId)
        {
            if (!User.HasAccessToPatient(patientId))
                return Forbid();

            var result = await _patientService.GetAllInvoicesByPatientIdAsync(patientId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpGet("{patientId:guid}/stays")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        public async Task<IActionResult> GetAllStaysByPatientIdAsync(Guid patientId)
        {
            if (!User.HasAccessToPatient(patientId))
                return Forbid();

            var result = await _patientService.GetAllStaysByPatientIdAsync(patientId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}