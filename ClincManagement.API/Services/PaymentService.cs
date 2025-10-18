using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Responses;
using ClincManagement.API.Errors;


namespace ClincManagement.API.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(ApplicationDbContext context, ILogger<PaymentService> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<Result<PaymentResponse>> PayWithVisaAsync(
            string userId,
            Guid appointmentId,
            VisaPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.Patient.UserId == userId, cancellationToken);

            if (appointment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

            var payment = new Payment
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                InvoiceId = Guid.NewGuid(),
                Amount = request.Amount,
                Method = PaymentMethod.Card,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            // محاكاة نجاح الدفع
            payment.Status = PaymentStatus.Success;
            payment.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Visa payment completed for appointment {AppointmentId} by user {UserId}", appointmentId, userId);

            return Result.Success(MapToPaymentResponse(payment));
        }


        public async Task<Result<PaymentResponse>> PayWithInstapayAsync(
            string userId,
            Guid appointmentId,
            InstapayPaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            var appointment = await _context.Appointments
                .FirstOrDefaultAsync(a => a.Id == appointmentId && a.Patient.UserId == userId, cancellationToken);

            if (appointment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

            var payment = new Payment
            {
                AppointmentId = appointmentId,
                PatientId = appointment.PatientId,
                InvoiceId = Guid.NewGuid(),
                Amount = request.Amount,
                Method = PaymentMethod.Instapay,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            // محاكاة نجاح الدفع بعد التأكيد
            payment.Status = PaymentStatus.Success;
            payment.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Instapay payment completed for appointment {AppointmentId} by user {UserId}", appointmentId, userId);

            return Result.Success(MapToPaymentResponse(payment));
        }


        public async Task<Result<PaymentResponse>> GetAsync(
            string userId,
            Guid paymentId,
            CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Patient.UserId == userId, cancellationToken);

            if (payment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

            return Result.Success(MapToPaymentResponse(payment));
        }


        public async Task<Result<PaymentResponse>> CancelPaymentAsync(
            Guid paymentId,
            string userId,
            CancellationToken cancellationToken = default)
        {
            var payment = await _context.Payments
                .Include(p => p.Patient)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Patient.UserId == userId, cancellationToken);

            if (payment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

            if (payment.Status == PaymentStatus.Success)
                return Result.Failure<PaymentResponse>(PaymentErrors.IsPaid);

            if (payment.Status == PaymentStatus.Failed)
                return Result.Failure<PaymentResponse>(PaymentErrors.Cancelled);

            payment.Status = PaymentStatus.Failed;
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Payment {PaymentId} cancelled by user {UserId}", paymentId, userId);

            return Result.Success(MapToPaymentResponse(payment));
        }
        private static PaymentResponse MapToPaymentResponse(Payment payment)
            => new PaymentResponse
            {
                Id = payment.Id,
                AppointmentId = payment.AppointmentId,
                InvoiceId = payment.InvoiceId,
                Method = payment.Method,
                Status = payment.Status,
                Amount = payment.Amount,
                CreatedAt = payment.CreatedAt,
                ConfirmedAt = payment.ConfirmedAt
            };
    }
}
