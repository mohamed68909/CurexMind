using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountsController(IAccountService accountService) : ControllerBase
    {
        private readonly IAccountService _accountService = accountService;

        [HttpGet]
        [ProducesResponseType(typeof(UserProfileResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] // User not found
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)] // User disabled
        public async Task<IActionResult> GetUserInfo()
        {
            var result = await _accountService.GetUserProfile(User.GetUserId());
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPut("info")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] // User not found
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)] // Update failed
        public async Task<IActionResult> UpdateUserInfo([FromBody] UpdateUserProfileRequest request)
        {
            var result = await _accountService.UpdateProfileAsync(User.GetUserId(), request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpPut("change-password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)] // Wrong current password
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)] // User not found
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await _accountService.ChangePasswordAsync(User.GetUserId(), request);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
