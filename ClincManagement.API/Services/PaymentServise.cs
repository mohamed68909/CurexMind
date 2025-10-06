//using ClincManagement.API.Abstractions;
//using ClincManagement.API.Contracts.Payment.Requests;
//using ClincManagement.API.Contracts.Payment.Respones;
//using ClincManagement.API.Errors;

//using ClincManagement.API.Data;
//using ClincManagement.API.Entities;
//using ClincManagement.API.Services.Gateways;
//using ClincManagement.API.Mapping;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using ClincManagement.API.Errors.ClincManagement.API.Errors;
//using ClincManagement.API.Services.Interface;

//namespace ClincManagement.API.Services
//{
//    public class PaymentService : IPaymentService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<PaymentService> _logger;
//        private readonly StripeCardGatewayService _stripeCardService;
//        private readonly InstapayGatewayService _instapayService;

//        public PaymentService(
//            ApplicationDbContext context,
//            ILogger<PaymentService> logger,
//            StripeCardGatewayService stripeCardService,
//            InstapayGatewayService instapayService)
//        {
//            _context = context;
//            _logger = logger;
//            _stripeCardService = stripeCardService;
//            _instapayService = instapayService;
//        }


//        public async Task<Result<PaymentTransactionResponse>> InitiatePaymentAsync(
//            string userId,
//            Guid appointmentId,
//            InitiatePaymentRequest request,
//            CancellationToken cancellationToken = default)
//        {

//            var appointment = await _context.Appointments
//                .Include(a => a.Patient).ThenInclude(p => p.User)
//                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.Patient.UserId == userId, cancellationToken);

//            if (appointment is null)
//                return Result.Failure<PaymentTransactionResponse>(AppointmentErrors.NotFound);

//            if (appointment.Status != AppointmentStatus.PendingPayment)
//                return Result.Failure<PaymentTransactionResponse>(PaymentErrors.AlreadyProcessed);

//            var newPayment = PaymentMapping.MapToPaymentEntity(request, appointment.PatientId, appointmentId);
//            _context.Payments.Add(newPayment);
//            await _context.SaveChangesAsync(cancellationToken);

//            var paymentResult = request.PaymentMethod.ToLowerInvariant() switch
//            {
//                "card" => await _stripeCardService.ProcessPaymentAsync(newPayment, request, cancellationToken),
//                "instapay" => await _instapayService.ProcessPaymentAsync(newPayment, request, cancellationToken),
//                "payatclinic" => await HandlePayAtClinicAsync(newPayment, appointment),
//                _ => Result.Failure<PaymentTransactionResponse>(PaymentErrors.InvalidMethod)
//            };

//            await _context.SaveChangesAsync(cancellationToken);

//            if (paymentResult.IsSuccess)
//            {
//                _logger.LogInformation("Payment {PaymentId} initiated successfully via {Method}", newPayment.Id, request.PaymentMethod);
//            }
//            return paymentResult;
//        }


//        public async Task<Result<ResponseConfirmPyment>> ConfirmPaymentAsync(
//            string userId,
//            RequestConfirmPayment request,
//            CancellationToken cancellationToken = default)
//        {

//            var payment = await _context.Payments
//                .Include(p => p.Appointment).ThenInclude(a => a.Doctor)
//                .Include(p => p.Appointment).ThenInclude(a => a.Patient).ThenInclude(pa => pa.User)
//                .FirstOrDefaultAsync(p => p.Id == request.PaymentId && p.Appointment.Patient.UserId == userId, cancellationToken);

//            if (payment is null)
//                return Result.Failure<ResponseConfirmPyment>(PaymentErrors.NotFound);

//            if (payment.Status != "RequiresConfirmation" && payment.Status != "AwaitingExternalConfirmation")
//                return Result.Failure<ResponseConfirmPyment>(PaymentErrors.ConfirmationNotRequired);


//            var confirmationResult = payment.Method.ToLowerInvariant() switch
//            {
//                "card" => await _stripeCardService.ConfirmPaymentAsync(payment, request, cancellationToken),
//                "instapay" => await _instapayService.ConfirmPaymentAsync(payment, request, cancellationToken),
//                _ => Result.Failure<ResponseConfirmPyment>(PaymentErrors.ConfirmationNotSupported)
//            };


//            if (confirmationResult.IsSuccess && payment.Status == "Success")
//            {
//                payment.Appointment.Status = AppointmentStatus.Confirmed;
//                await _context.SaveChangesAsync(cancellationToken);
//                _logger.LogInformation("Payment {PaymentId} confirmed successfully. Appointment {AppointmentId} is now confirmed.", payment.Id, payment.AppointmentId);
//            }

//            return confirmationResult;
//        }


//        public async Task<Result<PaymentTransactionResponse>> GetPaymentStatusAsync(
//            string userId,
//            Guid paymentId,
//            CancellationToken cancellationToken = default)
//        {
//            var payment = await _context.Payments
//                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Patient.UserId == userId, cancellationToken);

//            if (payment is null)
//                return Result.Failure<PaymentTransactionResponse>(PaymentErrors.NotFound);


//            return Result.Success(PaymentMapping.MapToPaymentResponse(payment));
//        }


//        public async Task<Result<PaymentTransactionResponse>> CancelPaymentAsync(
//            Guid paymentId,
//            string userId,
//            CancellationToken cancellationToken = default)
//        {
//            var payment = await _context.Payments
//                .Include(p => p.Appointment)
//                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Patient.UserId == userId, cancellationToken);

//            if (payment is null)
//                return Result.Failure<PaymentTransactionResponse>(PaymentErrors.NotFound);

//            if (payment.Status == "Success" || payment.Status == "Cancelled")
//                return Result.Failure<PaymentTransactionResponse>(PaymentErrors.CancellationNotAllowed);


//            payment.Status = "Cancelled";
//            payment.Appointment.Status = AppointmentStatus.Cancelled;
//            await _context.SaveChangesAsync(cancellationToken);

//            _logger.LogWarning("Payment {PaymentId} canceled by user {UserId}", paymentId, userId);
//            return Result.Success(PaymentMapping.MapToPaymentResponse(payment));
//        }

//        private async Task<Result<PaymentTransactionResponse>> HandlePayAtClinicAsync(
//            Payment payment, Appointment appointment)
//        {

//            payment.Status = "Success";
//            payment.Appointment.Status = AppointmentStatus.Confirmed;
//            payment.InvoiceId = Guid.NewGuid();

//            await _context.SaveChangesAsync();

//            _logger.LogInformation("Payment {PaymentId} marked as PayAtClinic.", payment.Id);
//            return Result.Success(PaymentMapping.MapToPaymentResponse(payment));
//        }
//    }
//}