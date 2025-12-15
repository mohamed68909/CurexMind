namespace ClincManagement.API.Entities
{
    public class Stay
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid PatientId { get; set; }

        public string Department { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;

        public StayType StayType { get; set; } = StayType.Inpatient;

        public StayStatus Status { get; set; } = StayStatus.Active;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }

        
        public Patient Patient { get; set; } = default!;

       
        public string RoomBed => $"{RoomNumber}-{BedNumber}";

    }
}
