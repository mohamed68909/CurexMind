using ClincManagement.API.Abstractions;
using ClincManagement.API.Abstractions.Consts;
using ClincManagement.API.Contracts.Invoice.Requests;
using ClincManagement.API.Contracts.Invoice.Responses;
using ClincManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClincManagement.API.Controllers
{
    [Route("api/invoices")]
    [ApiController]
    [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        
        [HttpGet]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(IEnumerable<InvoiceSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllInvoices()
        {
            var result = await _invoiceService.GetAllInvoicesAsync();
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpGet("{invoiceId:guid}")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(InvoiceDetailsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInvoiceDetails(Guid invoiceId)
        {
           
            var result = await _invoiceService.GetInvoiceDetailsAsync(invoiceId);
            return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
        }

       
        [HttpPost]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        [ProducesResponseType(typeof(InvoiceDetailsDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateInvoice([FromBody] CreateInvoiceDto request, CancellationToken cancel)
        {
            var result = await _invoiceService.CreateInvoiceAsync(request, cancel);

            return result.IsSuccess
                ? CreatedAtAction(nameof(GetInvoiceDetails), new { invoiceId = result.Value.InvoiceId }, result.Value)
                : result.ToProblem();
        }

        
        [HttpPut("{invoiceId:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        public async Task<IActionResult> UpdateInvoice(Guid invoiceId, [FromBody] UpdateInvoiceDto request, CancellationToken cancel)
        {
            var result = await _invoiceService.UpdateInvoiceAsync(invoiceId, request, cancel);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

       
        [HttpDelete("{invoiceId:guid}")]
        [Authorize(Roles = DefaultRoles.Admin.Name)]
        public async Task<IActionResult> DeleteInvoice(Guid invoiceId)
        {
            var result = await _invoiceService.DeleteInvoiceAsync(invoiceId);
            return result.IsSuccess ? NoContent() : result.ToProblem();
        }

    
        [HttpGet("{invoiceId:guid}/pdf")]
        [Authorize(Roles = $"{DefaultRoles.Admin.Name},{DefaultRoles.Patient.Name}")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> GeneratePdf(Guid invoiceId)
        {
           
            var result = await _invoiceService.GeneratePdfExportAsync(invoiceId);
            if (!result.IsSuccess) return result.ToProblem();

            return File(result.Value, "application/pdf", $"Invoice_{invoiceId}.pdf");
        }
    }
}