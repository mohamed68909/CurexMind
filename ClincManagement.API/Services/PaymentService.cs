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
        // Get all payments (ADMIN)
        // =========================
        public async Task<Result<IEnumerable<PaymentResponse>>> GetAllAsync(
            CancellationToken cancellationToken = default)
        {
            var payments = await _context.Payments
                .Include(p => p.Appointment)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return Result.Success(payments.Select(MapToPaymentResponse));
        }

        // =========================
        // Create payment
        // =========================
        public async Task<Result<PaymentResponse>> CreateAsync(
            string userId,
            Guid appointmentId,
            CreatePaymentRequest request,
            CancellationToken cancellationToken = default)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);

            if (appointment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

           
            var alreadyPaid = await _context.Payments
                .AnyAsync(p => p.AppointmentId == appointmentId && p.Status == PaymentStatus.Success, cancellationToken);

            if (alreadyPaid)
                return Result.Failure<PaymentResponse>(PaymentErrors.AlreadyProcessed);

            var payment = new Payment
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                InvoiceId = null,
                Amount = request.Amount,
                Method = request.Method,
                Status = request.Method == PaymentMethod.Wallet ? PaymentStatus.Success : PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                ConfirmedAt = request.Method == PaymentMethod.Wallet ? DateTime.UtcNow : null
            };

         
            appointment.PaymentStatus = payment.Status;
            appointment.Status = payment.Status == PaymentStatus.Success
                ? AppointmentStatus.Confirmed
                : AppointmentStatus.Waiting;

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Payment created: AppointmentId={AppointmentId}, UserId={UserId}, Amount={Amount}, Method={Method}, Status={Status}",
                appointmentId, userId, payment.Amount, payment.Method, payment.Status);

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
                .Include(p => p.Appointment)
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
                .Include(p => p.Appointment)
                .FirstOrDefaultAsync(p => p.Id == paymentId && p.Patient.UserId == userId, cancellationToken);

            if (payment is null)
                return Result.Failure<PaymentResponse>(PaymentErrors.NotFound);

            if (payment.Status == PaymentStatus.Success)
                return Result.Failure<PaymentResponse>(PaymentErrors.AlreadyPaid);

            if (payment.Status == PaymentStatus.Cancelled)
                return Result.Failure<PaymentResponse>(PaymentErrors.AlreadyCancelled);

            payment.Status = PaymentStatus.Cancelled;

            if (payment.Appointment != null)
            {
                payment.Appointment.PaymentStatus = PaymentStatus.Cancelled;
                payment.Appointment.Status = AppointmentStatus.Waiting;
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Payment cancelled: PaymentId={PaymentId}, UserId={UserId}, Amount={Amount}, Method={Method}",
                paymentId, userId, payment.Amount, payment.Method);

            return Result.Success(MapToPaymentResponse(payment));
        }

        // =========================
        // Mapper
        // =========================
        private static PaymentResponse MapToPaymentResponse(Payment payment)
        {
            return new PaymentResponse
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
}
