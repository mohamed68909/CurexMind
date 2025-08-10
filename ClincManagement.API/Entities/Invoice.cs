namespace ClincManagement.API.Entities
{
    public class Invoice
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string? Notes { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate{ get; set; } = DateTime.Now;
        public Patient Patient { get; set; } = default!;
    }
}
