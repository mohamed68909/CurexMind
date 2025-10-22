using ClinicManagement.API.Services.Interface;
using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Requests;
using ClincManagement.API.Contracts.Invoice.Respones;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet("All")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInvoices([FromQuery] InvoiceFilterDto? filterParams = null)
        {
            var result = await _invoiceService.GetAllInvoicesAsync(filterParams);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpGet("{invoiceId:guid}")]
        [ProducesResponseType(typeof(InvoiceDetailsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetInvoiceDetails([FromRoute] Guid invoiceId)
        {
            var result = await _invoiceService.GetInvoiceDetailsAsync(invoiceId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDetailsDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto request, CancellationToken cancel)
        {
            var result = await _invoiceService.CreateInvoiceAsync(request, cancel);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetInvoiceDetails), new { invoiceId = result.Value.InvoiceId }, result.Value)
                : result.ToProblem();
        }

        [HttpPut("{invoiceId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateInvoice([FromRoute] Guid invoiceId, [FromBody] UpdateInvoiceDto request, CancellationToken cancel)
        {
            var result = await _invoiceService.UpdateInvoiceAsync(invoiceId, request, cancel);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpDelete("{invoiceId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteInvoice([FromRoute] Guid invoiceId)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(invoiceId);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

        [HttpGet("GeneratePdf/{invoiceId:guid}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GeneratePdf([FromRoute] Guid invoiceId)
        {
            var result = await _invoiceService.GeneratePdfExportAsync(invoiceId);
            if (!result.IsSuccess)
                return result.ToProblem();

            return File(result.Value, "application/pdf", $"Invoice_{invoiceId}.pdf");
        }
    }
}
