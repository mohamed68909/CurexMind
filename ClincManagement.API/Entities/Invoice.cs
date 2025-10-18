using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{

    public class Invoice : Auditable
    {
        public Guid Id { get; set; }

        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid ServiceTypeId { get; set; }


        public decimal TotalAmountEGP { get; set; }
        public decimal DiscountEGP { get; set; }
        public decimal FinalAmountEGP { get; set; }
        public decimal PaidAmountEGP { get; set; }

        public string InvoiceNumber { get; set; } = string.Empty;
        public DateTime InvoiceDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;

        public DateTime VisitDate { get; set; }
        public string VisitTime { get; set; } = string.Empty;

        public string? Notes { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }


        public Patient Patient { get; set; } = default!;
        public Doctor Doctor { get; set; } = default!;
        public Clinic Clinic { get; set; } = default!;
        public ServiceType ServiceType { get; set; } = default!;
        public Payment? Payment { get; set; }
    }
}