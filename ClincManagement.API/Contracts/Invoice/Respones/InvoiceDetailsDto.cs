namespace ClincManagement.API.Contracts.Invoice.Responses
{
    public class InvoiceDetailsDto
    {
        public Guid InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PaymentStatus { get; set; }
        public string PaymentMethod { get; set; }
        public string ClinicName { get; set; }

        public PatientResponseDto Patient { get; set; }
        public DoctorResponseDto Doctor { get; set; }
        public ServiceResponseDto Service { get; set; }
        public AmountBreakdownDto AmountBreakdown { get; set; }

        public string Notes { get; set; }
    }

    public class PatientResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string NationalId { get; set; }
        public DateTime Birthdate { get; set; }
        public string Gender { get; set; }
    }

    public class DoctorResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
    }

    public class ServiceResponseDto
    {
        public string Type { get; set; }
        public DateTime VisitDate { get; set; }
        public string VisitTime { get; set; }
    }

    public class AmountBreakdownDto
    {
        public string ServiceDescription { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal Remaining { get; set; }
    }
}
