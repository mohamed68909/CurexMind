namespace ClincManagement.API.Contracts.Payment.Requests
{
    public record RequestConfirmPayment
    {
        public Guid PaymentId { get; set; }
        public string otpCode { get; set; } = string.Empty;
    }
}
