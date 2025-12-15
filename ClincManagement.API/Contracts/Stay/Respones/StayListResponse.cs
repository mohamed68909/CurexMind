namespace ClincManagement.API.Contracts.Stay.Responses
{
    public class StayListResponse
    {
        public Guid StayId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string RoomBed { get; set; } = string.Empty;
        public string StayType { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
