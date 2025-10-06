//using ClincManagement.API.Abstractions;
//using ClincManagement.API.Contracts.Payment.Requests;
//using ClincManagement.API.Contracts.Payment.Respones;
//using ClincManagement.API.Errors.ClincManagement.API.Errors;
//using ClincManagement.API.Services.Interface;

//namespace ClincManagement.API.Services.Gateways
//{
//    public class InstapayGatewayService : IPaymentGatewayService
//    {
//        private readonly ILogger<InstapayGatewayService> _logger;

//        public InstapayGatewayService(ILogger<InstapayGatewayService> logger)
//        {
//            _logger = logger;
//        }

//        public async Task<Result<PaymentTransactionResponse>> ProcessPaymentAsync(
//            Payment paymentEntity,
//            InitiatePaymentRequest request,
//            CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("Simulating Instapay request for {Amount}", paymentEntity.Amount);

//            paymentEntity.Status = "AwaitingExternalConfirmation";
//            paymentEntity.TransactionId = $"INSTAPAY_{Guid.NewGuid().ToString().Substring(0, 8)}";

//            return Result.Success(MapToPaymentResponse(paymentEntity, "Instapay request sent. Waiting for user approval on the Instapay application."));
//        }

//        public async Task<Result<ResponseConfirmPyment>> ConfirmPaymentAsync(
//            Payment paymentEntity,
//            RequestConfirmPayment request,
//            CancellationToken cancellationToken)
//        {
//            if (paymentEntity.Status == "AwaitingExternalConfirmation")
//            {
//                paymentEntity.Status = "Success";

//                return Result.Success(new ResponseConfirmPyment
//                {
//                    PaymentId = paymentEntity.Id,
//                    Status = paymentEntity.Status,
//                    Message = "Instapay confirmation simulated successfully."
//                });
//            }

//            return Result.Failure<ResponseConfirmPyment>(PaymentErrors.ConfirmationNotSupported);
//        }

//        private static PaymentTransactionResponse MapToPaymentResponse(Payment payment, string message) =>
//            new PaymentTransactionResponse(
//                payment.Id,
//                payment.AppointmentId,
//                payment.Amount,
//                payment.Currency,
//                payment.Method,
//                payment.Status,
//                payment.TransactionId ?? string.Empty,
//                message,
//                payment.CreatedAt
//            );
//    }
//}
