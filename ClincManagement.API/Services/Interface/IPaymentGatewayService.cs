using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Respones;

namespace ClincManagement.API.Services.Interface
{
    public interface IPaymentGatewayService
    {
        Task<Result<PaymentTransactionResponse>> ProcessPaymentAsync(
            Payment paymentEntity,
            InitiatePaymentRequest request,
            CancellationToken cancellationToken);

        Task<Result<ResponseConfirmPyment>> ConfirmPaymentAsync(
            Payment paymentEntity,
            RequestConfirmPayment request,
            CancellationToken cancellationToken);
    }
}
