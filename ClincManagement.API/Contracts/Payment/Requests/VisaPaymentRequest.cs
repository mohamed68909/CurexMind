namespace ClincManagement.API.Contracts.Payment.Requests
{
    public class VisaPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string CardNumber { get; set; } = string.Empty;
        public string ExpiryDate { get; set; } = string.Empty;
        public string CVV { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
