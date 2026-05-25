namespace ClinicManagement.API.Contracts.Invoice.Responses
{
    public class ResponsePatientInvoice
    {
        public string Date { get; set; } = string.Empty;
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
