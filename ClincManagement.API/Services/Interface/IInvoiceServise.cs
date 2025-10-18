using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Requests;
using ClincManagement.API.Contracts.Invoice.Requests.ClinicManagement.API.Services.Dtos;
using ClincManagement.API.Contracts.Invoice.Respones;
using ClincManagement.API.Contracts.Invoice.Respones.ClinicManagement.API.Services.Dtos;

namespace ClinicManagement.API.Services.Interface
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceData, CancellationToken cancellation);

        Task<Result<InvoiceDetailsDto>> GetInvoiceDetailsAsync(string invoiceId);

        Task<Result<InvoiceDetailsDto>> UpdateInvoiceAsync(string invoiceId, UpdateInvoiceDto updateData, CancellationToken cancellation);

        Task<Result> DeleteInvoiceAsync(string invoiceId);

        Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllInvoicesAsync(InvoiceFilterDto filterParams = null);

        Task<Result<byte[]>> GeneratePdfExportAsync(string invoiceId);
    }
}