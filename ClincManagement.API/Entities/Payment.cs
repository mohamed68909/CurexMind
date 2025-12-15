namespace ClincManagement.API.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid PatientId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid? InvoiceId { get; set; }

        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? TransactionId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ConfirmedAt { get; set; }

        // Navigation
        public Patient Patient { get; set; } = default!;
        public Appointment Appointment { get; set; } = default!;
        public Invoice? Invoice { get; set; }
    }
}
