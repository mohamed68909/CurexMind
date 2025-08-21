namespace ClincManagement.API.Entities
{
    public class VitalSigns
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId { get; set; }
        public int? BloodPressureSystolic { get; set; }
        public int? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? Weight { get; set; }
        public DateTime? RecordedDate { get; set; } = DateTime.Now;
        public string RecordedBy { get; set; } 
        public Patient Patient { get; set; } = null!;
        public ApplicationUser User { get; set; } = default!;

    }
}
