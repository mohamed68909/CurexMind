namespace ClincManagement.API.Contracts.Payment.Requests
{
    public record RequestCard
    {
        public Guid AppointmentId { get; set; }
       public PaymentMethod paymentMethod { get; set; } = PaymentMethod.Card;
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty; // MM/YY
        public string CVV { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string currency { get; set; }
    }
}
