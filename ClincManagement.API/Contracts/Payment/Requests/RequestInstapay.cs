namespace ClincManagement.API.Contracts.Payment.Requests
{
    public record RequestInstapay
    {
        public Guid AppointmentId { get; set; }
        public PaymentMethod paymentMethod { get; set; } = PaymentMethod.Instapay;
        public string PhoneNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PHP";
    }
}
