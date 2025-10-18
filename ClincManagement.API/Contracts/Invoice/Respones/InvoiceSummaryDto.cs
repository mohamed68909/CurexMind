namespace ClincManagement.API.Contracts.Invoice.Respones
{

    public class InvoiceSummaryDto
    {
        public string InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string PatientName { get; set; }
        public string DoctorName { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentStatus { get; set; }
    }
}

