namespace ClincManagement.API.Contracts.Stay.Requests
{
    public class UpdateStayDto
    {
        public List<Guid>? ServiceIds { get; set; }
        public string? Department { get; set; }
        public string? StayType { get; set; }
        public string? Status { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Notes { get; set; }
    }
}
