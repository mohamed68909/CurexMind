using ClinicManagement.API.Abstractions;
using ClinicManagement.API.Contracts.Payment.Requests;
using ClinicManagement.API.Contracts.Payment.Responses;

namespace ClinicManagement.API.Services;

public interface IPaymentService
{
    // Get all payments
    Task<Result<IEnumerable<PaymentResponse>>> GetAllAsync(
        CancellationToken cancellationToken = default);

    // Create payment (manual / cash / internal)
    Task<Result<PaymentResponse>> CreateAsync(
        string userId,
        Guid appointmentId,
        CreatePaymentRequest request,
        bool isAdmin = false,
        CancellationToken cancellationToken = default);

    // Get payment by id
    Task<Result<PaymentResponse>> GetAsync(
        string userId,
        Guid paymentId,
        bool isAdmin = false,
        CancellationToken cancellationToken = default);

    // Cancel payment
    Task<Result<PaymentResponse>> CancelPaymentAsync(
        Guid paymentId,
        string userId,
        CancellationToken cancellationToken = default);
}
