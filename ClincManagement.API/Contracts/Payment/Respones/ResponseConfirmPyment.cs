namespace ClincManagement.API.Contracts.Payment.Respones
{
    public record ResponseConfirmPyment
    {
        public Guid PaymentId { get; set; }
        public Guid AppointmentId { get; set; }
        public string doctorName { get; set; } = string.Empty;
        public string specialty { get; set; } = string.Empty;

        public PaymentMethod PaymentMethod { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "PHP";
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Message { get; set; } = "Payment confirmed successfully";
        public PatientDetalis PatientDetalis { get; set; } = new PatientDetalis();
    }
     
    public record PatientDetalis
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
