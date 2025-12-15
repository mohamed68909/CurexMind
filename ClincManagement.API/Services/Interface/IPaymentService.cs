using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Responses;

namespace ClincManagement.API.Services;

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
        CancellationToken cancellationToken = default);

    // Get payment by id
    Task<Result<PaymentResponse>> GetAsync(
        string userId,
        Guid paymentId,
        CancellationToken cancellationToken = default);

    // Cancel payment
    Task<Result<PaymentResponse>> CancelPaymentAsync(
        Guid paymentId,
        string userId,
        CancellationToken cancellationToken = default);
}
