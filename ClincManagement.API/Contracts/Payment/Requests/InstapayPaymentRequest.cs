namespace ClincManagement.API.Contracts.Payment.Requests
{

    public class InstapayPaymentRequest
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string InstapayId { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
