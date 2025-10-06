//using ClincManagement.API.Abstractions;
//using ClincManagement.API.Contracts.Payment.Requests;
//using ClincManagement.API.Contracts.Payment.Respones;
//using ClincManagement.API.Errors.ClincManagement.API.Errors;
//using ClincManagement.API.Services.Interface;
//using ClincManagement.API.Settings;
//using Microsoft.Extensions.Options;

//namespace ClincManagement.API.Services.Gateways
//{
//    public class StripeCardGatewayService : IPaymentGatewayService
//    {
//        private readonly PaymentGatewaySettings _options;
//        private readonly ILogger<StripeCardGatewayService> _logger;

//        public StripeCardGatewayService(
//            IOptions<PaymentGatewaySettings> options,
//            ILogger<StripeCardGatewayService> logger)
//        {
//            _options = options.Value;
//            _logger = logger;

//            StripeConfiguration.ApiKey = _options.SecretKey;
//        }

//        public async Task<Result<PaymentTransactionResponse>> ProcessPaymentAsync(
//            Payment paymentEntity,
//            InitiatePaymentRequest request,
//            CancellationToken cancellationToken)
//        {
//            try
//            {
//                var service = new PaymentIntentService();
//                long amountInCents = (long)(paymentEntity.Amount * 100);

//                var options = new PaymentIntentCreateOptions
//                {
//                    Amount = amountInCents,
//                    Currency = _options.Currency.ToLowerInvariant(),
//                    PaymentMethod = request.CardNumber,
//                    ConfirmationMethod = "manual",
//                    Confirm = true,
//                    Description = $"Appointment {paymentEntity.AppointmentId} fee",
//                };

//                var intent = await service.CreateAsync(options, cancellationToken: cancellationToken);

//                paymentEntity.TransactionId = intent.Id;
//                paymentEntity.Status = MapStripeStatusToInternal(intent.Status);

//                return Result.Success(MapToPaymentResponse(paymentEntity, "Payment initiated. Check status for confirmation."));
//            }
//            catch (StripeException ex)
//            {
//                _logger.LogError(ex, "Stripe payment failed for payment {Id}: {Message}", paymentEntity.Id, ex.Message);
//                paymentEntity.Status = "Failed";
//                return Result.Failure<PaymentTransactionResponse>(PaymentErrors.GatewayFailed);
//            }
//        }

//        public async Task<Result<ResponseConfirmPyment>> ConfirmPaymentAsync(
//            Payment paymentEntity,
//            RequestConfirmPayment request,
//            CancellationToken cancellationToken)
//        {
//            if (paymentEntity.TransactionId is null)
//                return Result.Failure<ResponseConfirmPyment>(PaymentErrors.NotFound);

//            paymentEntity.Status = "Success";

//            return Result.Success(new ResponseConfirmPyment
//            {
//                PaymentId = paymentEntity.Id,
//                Status = paymentEntity.Status,
//                Message = "Card payment confirmed successfully."
//            });
//        }

//        private static string MapStripeStatusToInternal(string stripeStatus) =>
//            stripeStatus switch
//            {
//                "succeeded" => "Success",
//                "requires_action" => "RequiresConfirmation",
//                "requires_payment_method" => "Failed",
//                _ => "Pending"
//            };

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
