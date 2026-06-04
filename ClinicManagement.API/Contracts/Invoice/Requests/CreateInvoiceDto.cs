namespace ClinicManagement.API.Contracts.Invoice.Requests
{
    public class CreateInvoiceDto
    {
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }

        public ServiceDetailsDto ServiceDetails { get; set; } = default!;
        public AmountDetailsDto AmountDetails { get; set; } = default!;
        public PaymentInformationDto PaymentInformation { get; set; } = default!;

        public string? Notes { get; set; }
    }

    public class ServiceDetailsDto
    {
        public Guid ServiceTypeId { get; set; }
        public DateTime VisitDate { get; set; }
        public string ClinicName { get; set; } = string.Empty;
    }

    public class AmountDetailsDto
    {
        public decimal TotalAmountEGP { get; set; }
        public decimal DiscountEGP { get; set; }
        public decimal FinalAmountEGP { get; set; }
    }

    public class PaymentInformationDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public decimal AmountPaidEGP { get; set; }
    }
}
