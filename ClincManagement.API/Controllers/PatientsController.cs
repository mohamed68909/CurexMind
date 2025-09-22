using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Patient.Requests;
using ClincManagement.API.Contracts.Patient.Respones;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
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
       
        [ProducesResponseType(typeof(PatientService), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] 
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)] 
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreatePatientAsync([FromForm] PatientRequestDto request,CancellationToken cancellationToken)
        {
            var result = await _patientService.CreateAsync(User.GetUserId(), request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeletePatientAsync(Guid id, CancellationToken cancellationToken)
        {
            var result = await _patientService.Delete(id, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ToProblem();
        }






    }
}
