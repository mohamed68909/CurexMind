using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Requests;

using ClincManagement.API.Contracts.Invoice.Respones;


namespace ClinicManagement.API.Services.Interface
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceDetailsDto>> CreateInvoiceAsync(CreateInvoiceDto invoiceData, CancellationToken cancellation);

        Task<Result<InvoiceDetailsDto>> GetInvoiceDetailsAsync(Guid invoiceId);

        Task<Result<InvoiceDetailsDto>> UpdateInvoiceAsync(Guid invoiceId, UpdateInvoiceDto updateData, CancellationToken cancellation);

        Task<Result> DeleteInvoiceAsync(Guid invoiceId);

        Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllInvoicesAsync(InvoiceFilterDto filterParams = null);

        Task<Result<byte[]>> GeneratePdfExportAsync(Guid invoiceId);
    }
}