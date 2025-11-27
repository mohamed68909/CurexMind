using ClincManagement.API.Contracts.Invoice.Respones;
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
  
    public class PatientsController : ControllerBase
    {
        readonly IPatientService _patientService;
        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedPatientResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetPatientsAsync(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _patientService.GetPatientsAsync(search, page, pageSize);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPatientById(Guid id)
        {
            var result = await _patientService.GetPatientByIdAsync(id);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]

        [ProducesResponseType(typeof(PatientCreateResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePatientAsync([FromForm] PatientRequestDto request, CancellationToken cancellationToken)
        {
            var createdByUserId = User.GetUserId();
            var result = await _patientService.CreateAsync(createdByUserId,request.ProfileImageUrl, request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPatientById), new { id = result.Value.PatientId }, result.Value)
                : result.ToProblem();
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(PatientResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePatientAsync(Guid id, [FromForm] PatientRequestDto request)
        {
            var result = await _patientService.UpdatePatientAsync(id, request.ProfileImageUrl,request);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpDelete("{id:guid}")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePatientAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _patientService.Delete(id, cancellationToken);
            return result.IsSuccess? Ok(new { message = "Patient deleted successfully" }) : result.ToProblem();
        }

        //[HttpGet("{patientId}/appointments")]
        //[ProducesResponseType(typeof(IEnumerable<ResponseAllAppointmentPatient>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetAllAppointmentsByPatientIdAsync(Guid patientId)
        //{
        //    var result = await _patientService.GetAllAppointmentsByPatientIdAsync(patientId);
        //    return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        //}

        //[HttpGet("{patientId}/invoices")]
        //[ProducesResponseType(typeof(IEnumerable<ResponsePatientInvoice>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetAllInvoicesByPatientIdAsync(Guid patientId)
        //{
        //    var result = await _patientService.GetAllInvoicesByPatientIdAsync(patientId);
        //    return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        //}

        //[HttpGet("{patientId}/stays")]
        //[ProducesResponseType(typeof(IEnumerable<ResponsePatientStay>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetAllStaysByPatientIdAsync(Guid patientId)
        //{
        //    var result = await _patientService.GetAllStaysByPatientIdAsync(patientId);
        //    return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        //}

        //[HttpGet("{patientId}/operations")]
        //[ProducesResponseType(typeof(IEnumerable<ResponsePatientOperation>), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetAllOperationsByPatientIdAsync(Guid patientId)
        //{
        //    var result = await _patientService.GetAllOperationsByPatientIdAsync(patientId);
        //    return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        //}
    }
}