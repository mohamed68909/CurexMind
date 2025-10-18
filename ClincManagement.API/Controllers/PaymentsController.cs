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
        readonly IPaymentService _paymentService;
        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        [HttpPost("visa")]

        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PayWithVisa(
       [FromRoute] Guid appointmentId,
       [FromBody] VisaPaymentRequest request,
       CancellationToken cancellationToken)
        {
            var result = await _paymentService.PayWithVisaAsync(
                User.GetUserId(),
                appointmentId,
                request,
                cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPayment), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        [HttpPost("instapay")]

        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PayWithInstapay(
            [FromRoute] Guid appointmentId,
            [FromBody] InstapayPaymentRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _paymentService.PayWithInstapayAsync(
                User.GetUserId(),
                appointmentId,
                request,
                cancellationToken);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetPayment), new { id = result.Value.Id }, result.Value)
                : result.ToProblem();
        }

        [HttpGet("~/payments/{id}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPayment([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _paymentService.GetAsync(User.GetUserId(), id, cancellationToken);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpDelete("~/payments/{id}/cancel")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Cancel([FromRoute] Guid id, CancellationToken cancellationToken)
        {
            var result = await _paymentService.CancelPaymentAsync(id, User.GetUserId(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }
    }
}

