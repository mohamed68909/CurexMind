using ClincManagement.API.Abstractions;
using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Responses;
using ClincManagement.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpGet]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPayments(CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetAllAsync(cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpPost("{appointmentId:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreatePayment(
            [FromRoute] Guid appointmentId,
            [FromBody] CreatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _paymentService.CreateAsync(
                userId,
                appointmentId,
                request,
                cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPayment), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        
        [HttpGet("{id:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _paymentService.GetAsync(userId, id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

   
        [HttpDelete("{id:guid}/cancel")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> CancelPayment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _paymentService.CancelPaymentAsync(id, User.GetUserId(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}