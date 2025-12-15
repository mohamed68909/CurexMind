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
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ----------------------------------------
        // Get All Payments
        // ----------------------------------------
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllPayments(CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetAllAsync(cancellationToken);
            return result.IsSuccess
                ? Ok(result.Value)
                : StatusCode(StatusCodes.Status500InternalServerError, result.Error);
        }

        // ----------------------------------------
        // Create Payment (Manual / Cash / Card)
        // ----------------------------------------
        [HttpPost("{appointmentId}")]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreatePayment(
            [FromRoute] Guid appointmentId,
            [FromBody] CreatePaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.CreateAsync(
                User.GetUserId(),
                appointmentId,
                request,
                cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPayment), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        // ----------------------------------------
        // Get Payment By Id
        // ----------------------------------------
        [HttpGet("{id}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPayment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetAsync(User.GetUserId(), id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        // ----------------------------------------
        // Cancel Payment
        // ----------------------------------------
        [HttpDelete("{id}/cancel")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelPayment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _paymentService.CancelPaymentAsync(id, User.GetUserId(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}
