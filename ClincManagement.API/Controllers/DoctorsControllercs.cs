using ClincManagement.API.Contracts.Clinic.Respones;
using ClincManagement.API.Contracts.Doctors.Respones;
using ClincManagement.API.Contracts.Review.Requests;
using ClincManagement.API.Contracts.Review.Respones;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Doctors : ControllerBase
    {
        private readonly IDoctorService _doctorService;

        public Doctors(IDoctorService doctorService)
        {
            _doctorService = doctorService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DoctorListResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            var result = await _doctorService.GetAll(cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(DoctorDetailsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _doctorService.GetAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost("{doctorId:guid}/reviews")]
        [Authorize]
        [ProducesResponseType(typeof(AddReviewResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddReview(
            [FromRoute] Guid doctorId,
            [FromBody] AddReviewRequest request,
            CancellationToken cancellationToken)
        {
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Problem("User not authenticated", statusCode: StatusCodes.Status401Unauthorized);

            var result = await _doctorService.AddReview(doctorId, userId, request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetAsync), new { id = doctorId }, result.Value)
                : result.ToProblem();
        }
    }
}
