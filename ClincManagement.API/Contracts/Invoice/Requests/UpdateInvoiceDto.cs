namespace ClincManagement.API.Contracts.Invoice.Requests
{

    public class UpdateInvoiceDto
    {

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }

        public ServiceDetailsDto ServiceDetails { get; set; }
        public AmountDetailsDto AmountDetails { get; set; }
        public PaymentInformationDto PaymentInformation { get; set; }

        public string Notes { get; set; }
    }
}

