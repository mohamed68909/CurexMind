using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Invoice.Requests;

using ClincManagement.API.Contracts.Invoice.Responses;

namespace ClincManagement.API.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<Result<InvoiceDetailsDto>> CreateInvoiceAsync(
            CreateInvoiceDto invoiceData,
            CancellationToken cancellationToken = default
        );

        Task<Result<InvoiceDetailsDto>> GetInvoiceDetailsAsync(
            Guid invoiceId,
            CancellationToken cancellationToken = default
        );

        Task<Result<InvoiceDetailsDto>> UpdateInvoiceAsync(
            Guid invoiceId,
            UpdateInvoiceDto updateData,
            CancellationToken cancellationToken = default
        );

        Task<Result> DeleteInvoiceAsync(
            Guid invoiceId,
            CancellationToken cancellationToken = default
        );

        Task<Result<IEnumerable<InvoiceSummaryDto>>> GetAllInvoicesAsync(
         
            CancellationToken cancellationToken = default
        );

        Task<Result<byte[]>> GeneratePdfExportAsync(
            Guid invoiceId,
            CancellationToken cancellationToken = default
        );
    }
}
