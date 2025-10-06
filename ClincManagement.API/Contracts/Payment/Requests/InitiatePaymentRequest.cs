namespace ClincManagement.API.Contracts.Payment.Requests
{
    public record InitiatePaymentRequest
    {
        public Guid AppointmentId { get; set; }
       
        public string PaymentMethod { get; set; } = string.Empty; // "Card", "Instapay", "PayAtClinic" 

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "EGP";

     
        public string? CardNumber { get; set; }
        public string? CardHolderName { get; set; }
        public string? ExpiryDate { get; set; } // MM/YY
        public string? CVV { get; set; }


        public string? AccountIdentifier { get; set; } // قد يكون رقم الهاتف لـ Instapay
    }
}
