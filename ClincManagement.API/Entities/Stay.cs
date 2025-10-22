using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class Stay : Auditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid PatientId { get; set; }
        public Guid ServiceTypeId { get; set; }

        public string Department { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;

        public StayStatus Status { get; set; } = StayStatus.Admitted;

        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }

        public decimal TotalCost { get; set; } = decimal.Zero;
        public string? Notes { get; set; }

        
        public Patient Patient { get; set; } = default!;
        public ServiceType ServiceType { get; set; } = default!;

        public ICollection<StayActivity> ActivityLog { get; set; } = new List<StayActivity>();
        public ICollection<MedicalService> MedicalServices { get; set; } = new List<MedicalService>();
    }

    public enum StayStatus
    {
        Admitted,
        InProgress,
        Discharged,
        Cancelled
    }
}
