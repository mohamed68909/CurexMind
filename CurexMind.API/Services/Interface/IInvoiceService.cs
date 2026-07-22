using CurexMind.API.Abstractions;
using CurexMind.API.Contracts.Invoice.Requests;

using CurexMind.API.Contracts.Invoice.Responses;

namespace CurexMind.API.Services.Interfaces
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
            Guid? patientId = null,
            CancellationToken cancellationToken = default
        );
    }
}
