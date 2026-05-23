using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Abstractions.Consts;
using ClinicManagement.API.Contracts.Dashboard;
using ClinicManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ClinicManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name}")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet("receptionist-summary/{userId}")]
        [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetReceptionistSummary(Guid userId)
        {
            var result = await _dashboardService.GetReceptionistSummaryAsync(userId);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return StatusCode(result.Error.StatusCode, new
            {
                result.Error.Code,
                result.Error.Message
            });
        }
    }
}
