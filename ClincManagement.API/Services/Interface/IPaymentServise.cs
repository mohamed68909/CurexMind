using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Respones;

namespace ClincManagement.API.Services.Interface
{
   
    public interface IPaymentService
    {
       
        Task<Result<PaymentTransactionResponse>> InitiatePaymentAsync(  string userId,  Guid appointmentId,  InitiatePaymentRequest request, CancellationToken cancellationToken = default);

      
        Task<Result<ResponseConfirmPyment>> ConfirmPaymentAsync(string userId, RequestConfirmPayment request,  CancellationToken cancellationToken = default);

      
        Task<Result<PaymentTransactionResponse>> GetPaymentStatusAsync( string userId,Guid paymentId, CancellationToken cancellationToken = default);

     
        Task<Result<PaymentTransactionResponse>> CancelPaymentAsync( Guid paymentId, string userId, CancellationToken cancellationToken = default);
    }
}
