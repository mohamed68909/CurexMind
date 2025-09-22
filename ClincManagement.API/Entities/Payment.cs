namespace ClincManagement.API.Entities
{
    public class Payment
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty; // Instapay, Card, Wallet
        public PaymentStatus Status { get; set; }  // Pending, Success, Failed
        public string? TransactionId { get; set; }
        public DateTime CreatedAt { get; set; } 
        public DateTime? ConfirmedAt { get; set; }

        public Appointment Appointment { get; set; } = default!;
        public Invoice Invoice { get; set; } = default!;
    }
}
