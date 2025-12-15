namespace ClincManagement.API.Contracts.Patient.Respones
{
    public class ResponsePatientStay
    {
        public string RoomBed { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public decimal TotalCost { get; set; }
    }

}
