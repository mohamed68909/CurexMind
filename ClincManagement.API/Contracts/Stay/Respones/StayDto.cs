namespace ClincManagement.API.Contracts.Stay.Respones
{
    public class StayDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty; // للاستخدام في العرض
        public string Department { get; set; } = string.Empty;
        public string StayType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Notes { get; set; }
        public List<ActivityLogDto> ActivityLog { get; set; } = new List<ActivityLogDto>();
    }
    public class PagedStayResponse
    {
        public IEnumerable<StayDto> Data { get; set; } = new List<StayDto>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    
    public class ActivityLogDto
    {
        public Guid Id { get; set; }
        public string Action { get; set; } = string.Empty;
        public string ByUser { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
