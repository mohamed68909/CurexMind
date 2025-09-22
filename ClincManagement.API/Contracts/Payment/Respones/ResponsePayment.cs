namespace ClincManagement.API.Contracts.Payment.Respones
{
    public record ResponsePayment
    {
        public Guid PaymentId { get; set; }
        public Guid AppointmentId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PHP";
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = "Payment request created successfully";

    }
}
