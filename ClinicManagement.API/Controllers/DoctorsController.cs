using ClinicManagement.API.Abstractions.Consts;
using ClinicManagement.API.Contracts.Clinic.Respones;
using ClinicManagement.API.Contracts.Doctors.Requests;
using ClinicManagement.API.Contracts.Doctors.Respones;
using ClinicManagement.API.Contracts.Review.Requests;
using ClinicManagement.API.Contracts.Review.Respones;
using ClinicManagement.API.Services.Interface;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinicManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _doctorService;
        private readonly IValidator<CreateDoctorRequest> _createValidator;
        private readonly IValidator<UpdateDoctorRequest> _updateValidator;

        public DoctorsController(
            IDoctorService doctorService,
            IValidator<CreateDoctorRequest> createValidator,
            IValidator<UpdateDoctorRequest> updateValidator)
        {
            _doctorService = doctorService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedDoctorResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _doctorService.GetAll(page, pageSize, search, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DoctorDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _doctorService.GetAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(DoctorDetailsResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateDoctor(
            [FromForm] CreateDoctorRequest request,
            CancellationToken cancellationToken)
        {
            // Validate request using FluentValidation before calling the service
            var validation = await _createValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            var result = await _doctorService.CreateAsync(request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        
        [HttpPut("{id:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(DoctorDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDoctor(
            Guid id,
            [FromForm] UpdateDoctorRequest request,
            CancellationToken cancellationToken)
        {
            // Validate request using FluentValidation before calling the service
            var validation = await _updateValidator.ValidateAsync(request, cancellationToken);
            if (!validation.IsValid)
                return ValidationProblem(new ValidationProblemDetails(validation.ToDictionary()));

            var result = await _doctorService.UpdateAsync(id, request, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpDelete("{id:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDoctor(Guid id, CancellationToken cancellationToken)
        {
            var result = await _doctorService.DeleteAsync(id, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

      
        [HttpPost("{doctorId:guid}/reviews")]
        [Authorize(Roles = DefaultRoles.Patient.Name)]
        [ProducesResponseType(typeof(ReviewResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> AddReview(
            Guid doctorId,
            [FromBody] AddReviewRequest request,
            CancellationToken cancellationToken)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var result = await _doctorService.AddReview(
                doctorId,
                userId,
                request,
                cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = doctorId }, result.Value)
                : result.ToProblem();
        }
    }
}