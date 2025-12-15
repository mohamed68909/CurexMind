namespace ClincManagement.API.Contracts.Invoice.Responses
{
    public class ResponsePatientInvoice
    {
        public string Date { get; set; }
        public string InvoiceNumber { get; set; }
        public decimal Amount { get; set; }
        public decimal Paid { get; set; }
        public decimal Remaining { get; set; }
        public string Status { get; set; }
    }
}
