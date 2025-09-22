using ClincManagement.API.Contracts.Stay.Requests;
using ClincManagement.API.Contracts.Stay.Respones;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StayController : ControllerBase
    {
        private readonly IStayService _stayService;

        public StayController(IStayService stayService)
        {
            _stayService = stayService;
        }

        public StayController()
        {
        }

        [HttpPost]
      
        [ProducesResponseType(typeof(StayDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateStay([FromBody] CreateStayDto request, CancellationToken cancellationToken)
        {
            var result = await _stayService.CreateStayAsync(request, cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetStayById), new { stayId = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

    
        [HttpPut("{stayId:guid}")]
    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStay(Guid stayId, [FromBody] UpdateStayDto request, CancellationToken cancellationToken)
        {
            var result = await _stayService.UpdateStayAsync(stayId, request, cancellationToken);

            return result.IsSuccess ? Ok() : result.ToProblem();
        }

   
        [HttpDelete("{stayId:guid}")]
       
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStay(Guid stayId, CancellationToken cancellationToken)
        {
            var result = await _stayService.DeleteStayAsync(stayId, cancellationToken);

            return result.IsSuccess ? NoContent() : result.ToProblem();
        }


        [HttpGet("{stayId:guid}")]
        [ProducesResponseType(typeof(StayDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStayById(Guid stayId, CancellationToken cancellationToken)
        {
            var result = await _stayService.GetStayByIdAsync(stayId, cancellationToken);

            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        
        [HttpGet]
        [ProducesResponseType(typeof(PagedStayResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
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
