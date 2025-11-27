using System.ComponentModel.DataAnnotations;

namespace ClincManagement.API.Contracts.Stay.Requests
{
    public class CreateStayRequest
    {
        public Guid PatientId { get; set; }

        public string Department { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty;

        public StayType StayType { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Notes { get; set; }
    }

}
