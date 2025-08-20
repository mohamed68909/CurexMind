using ClincManagement.API.Contracts.Authentications.Requests;
using ClincManagement.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authServices) : ControllerBase
    {
        private readonly IAuthService _authServices = authServices;
        [HttpPost("sign-up")]
        public async Task<Result<IActionResult>>SignUp([FromBody] SignUpRequest request, CancellationToken cancellationToken)
        {
            var result = await _authServices.SignUpPatientAsync(request, cancellationToken);
            return ;
        }
    }
}
