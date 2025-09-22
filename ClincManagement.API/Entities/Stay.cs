using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class Stay : Auditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId { get; set; }
        public string Department { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty ;
        public string Status { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
      public  DateTime? CheckOutDate { get; set; }
        public string? Services { get; set; }
        public decimal TotalCost { get; set; } = decimal.Zero;
        public ICollection<StayActivity> ActivityLog { get; set; } = default!;

        public string? Notes { get; set; } 
        public Patient Patient { get; set; } = default!;


    }
}
