namespace ClincManagement.API.Contracts.Payment.Respones
{
    public record PaymentTransactionResponse
    {
        public Guid PaymentId { get; set; }
        public Guid AppointmentId { get; set; }
        public string PaymentMethod { get; set; } 
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string Status { get; set; } 
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = "Payment request processed successfully";
        public string? TransactionReference { get; set; }
    }
}
