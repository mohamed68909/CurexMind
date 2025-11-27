using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class Invoice : Auditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid ServiceTypeId { get; set; }
        public Guid? PaymentId { get; set; }

        public decimal TotalAmountEGP { get; set; } = 0;
        public decimal DiscountEGP { get; set; } = 0;
        public decimal FinalAmountEGP { get; set; } = 0;
        public decimal PaidAmountEGP { get; set; } = 0;

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime VisitDate { get; set; }
        public TimeOnly VisitTime { get; set; }   // استخدم TimeOnly لو .NET 6+

        public string? Notes { get; set; }
        public DateTime? DueDate { get; set; }

        // العلاقات
        public Patient Patient { get; set; } = default!;
        public Doctor Doctor { get; set; } = default!;
        public Clinic Clinic { get; set; } = default!;
    //    public ServiceType ServiceType { get; set; } = default!;
        public Payment? Payment { get; set; }
    }
}
