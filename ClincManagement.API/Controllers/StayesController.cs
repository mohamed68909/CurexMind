using ClincManagement.API.Abstractions;
using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public class StayController : ControllerBase
    {
        private readonly IStayService _stayService;

        public StayController(IStayService stayService)
        {
            _stayService = stayService;
        }

        
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(StayDetailsResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateStay([FromBody] CreateStayRequest request, CancellationToken cancellationToken)
        {
            var result = await _stayService.CreateStayAsync(request, cancellationToken);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetStayById), new { stayId = result.Value.StayId }, result.Value)
                : result.ToProblem();
        }

       
        [HttpPut("{stayId:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateStay(Guid stayId, [FromBody] UpdateStayRequest request, CancellationToken cancellationToken)
        {
            var result = await _stayService.UpdateStayAsync(stayId, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

       
        [HttpDelete("{stayId:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteStay(Guid stayId, CancellationToken cancellationToken)
        {
            var result = await _stayService.DeleteStayAsync(stayId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

       
        [HttpGet("{stayId:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(StayDetailsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStayById(Guid stayId, CancellationToken cancellationToken)
        {
            
            var result = await _stayService.GetStayByIdAsync(stayId, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpGet]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PagedStayResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllStays(
            [FromQuery] string? department,
            [FromQuery] string? status,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var result = await _stayService.GetAllStaysAsync(department, status, page, pageSize, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }
    }
}