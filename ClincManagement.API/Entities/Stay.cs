namespace ClincManagement.API.Entities
{
    public class Stay
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string BedNumber { get; set; } = string.Empty ;
        public DateTime CheckInDate { get; set; }
      public  DateTime? CheckOutDate { get; set; }
        public string? Services { get; set; }
        public decimal TotalCost { get; set; } = decimal.Zero;

        public string? Notes { get; set; } 
        public bool IsActive { get; set; }=true;
        public Patient Patient { get; set; } = default!;


    }
}
