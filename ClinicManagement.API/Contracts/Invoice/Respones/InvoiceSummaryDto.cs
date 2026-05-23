public class InvoiceSummaryDto
{
    public Guid InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }

    public string PatientName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;

    public string ServiceType { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
    public decimal DiscountApplied { get; set; }
    public decimal NetTotal { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
}
