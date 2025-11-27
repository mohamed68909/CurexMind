namespace ClincManagement.API.Contracts.Stay.Respones
{
    public class StayDetailsResponse
    {
        public Guid StayId { get; set; }

        public PatientInfoDto Patient { get; set; } = default!;
        public StayInfoDto Stay { get; set; } = default!;
    }

    public class PatientInfoDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
    }

    public class StayInfoDto
    {
        public string Department { get; set; } = string.Empty;
        public string RoomBed { get; set; } = string.Empty;
        public string StayType { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }

}
