using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Responses;




namespace ClincManagement.API.Services
{
    public interface IPaymentService
    {

        Task<Result<PaymentResponse>> PayWithVisaAsync(
            string userId,
            Guid appointmentId,
            VisaPaymentRequest request,
            CancellationToken cancellationToken = default);


        Task<Result<PaymentResponse>> PayWithInstapayAsync(
            string userId,
            Guid appointmentId,
            InstapayPaymentRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<PaymentResponse>> GetAsync(
            string userId,
            Guid paymentId,
            CancellationToken cancellationToken = default);





        Task<Result<PaymentResponse>> CancelPaymentAsync(
            Guid id,
            string userId,
            CancellationToken cancellationToken = default);
    }
}


