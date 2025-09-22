namespace ClincManagement.API.Contracts.Stay.Requests
{
    public class UpdateStayDto
    {
        public string? Department { get; set; }
        public string? StayType { get; set; }
        public string? Status { get; set; }
        public string? RoomNumber { get; set; }
        public string? BedNumber { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Notes { get; set; }
    }
}
