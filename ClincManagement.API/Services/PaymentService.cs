using ClincManagement.API.Abstractions;
using ClincManagement.API.Contracts.Payment.Requests;
using ClincManagement.API.Contracts.Payment.Responses;
using ClincManagement.API.Entities;
using ClincManagement.API.Errors;
using ClincManagement.API.Services.Interface;
using Microsoft.EntityFrameworkCore;

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

        // =========================
        // Get all payments
        // =========================
        public async Task<Result<IEnumerable<PaymentResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var payments = await _context.Payments
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(payments.Select(MapToPaymentResponse));
        }

        // =========================
        // Create payment for appointment
        // =========================
        public async Task<Result<PaymentResponse>> CreateAsync(
            string userId,
            Guid appointmentId,
            CreatePaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            // التحقق من وجود الحجز للمستخدم
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId , cancellationToken);

            if (appointment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

            // إنشاء سجل الدفع
            var payment = new Payment
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                InvoiceId = null, // لا توجد فاتورة مسبقة
                Amount = request.Amount,
                Method = request.Method,
                Status = PaymentStatus.Success,  // الدفع تم بنجاح يدويًا
                CreatedAt = DateTime.UtcNow,
                ConfirmedAt = DateTime.UtcNow
            };

            // تحديث حالة الحجز بعد الدفع
            appointment.Status = AppointmentStatus.Confirmed;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Payment created manually for appointment {AppointmentId} by user {UserId}",
                appointmentId, userId);

            return Result.Success(MapToPaymentResponse(payment));
        }

        // =========================
        // Get payment by id
        // =========================
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

        // =========================
        // Cancel payment
        // =========================
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

            _logger.LogInformation(
                "Payment {PaymentId} cancelled by user {UserId}",
                paymentId, userId);

            return Result.Success(MapToPaymentResponse(payment));
        }

        // =========================
        // Mapper
        // =========================
        private static PaymentResponse MapToPaymentResponse(Payment payment) =>
            new PaymentResponse
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
