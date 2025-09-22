using System.ComponentModel.DataAnnotations;

namespace ClincManagement.API.Contracts.Stay.Requests
{
    public class CreateStayDto
    {
        [Required]
        public Guid PatientId { get; set; }
        [Required]
        public string Department { get; set; } = string.Empty;
        [Required]
        public string StayType { get; set; } = string.Empty;
        [Required]
        public string RoomNumber { get; set; } = string.Empty;
        [Required]
        public string BedNumber { get; set; } = string.Empty;
        [Required]
        public string Status { get; set; } = "Active"; // القيمة الافتراضية "نشط"
        [Required]
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public string? Notes { get; set; }
    }
}
