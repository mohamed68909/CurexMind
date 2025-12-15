namespace ClincManagement.API.Contracts.Patient.Respones
{
    public class ResponsePatientInvoice
    {
        public string InvoiceNumber { get; set; }
        public string InvoiceDate { get; set; }
        public decimal FinalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public string Status { get; set; }
    }
}
