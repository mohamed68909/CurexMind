namespace ClincManagement.API.Contracts.Payment.Responses
{
    public class PaymentResponse
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid? InvoiceId { get; set; }
        public PaymentMethod Method { get; set; } = default!;
        public PaymentStatus Status { get; set; } = default!;
        public decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
    }
}
