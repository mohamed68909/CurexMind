namespace ClincManagement.API.Contracts.Stay.Responses
{
    public class ResponsePatientStay
    {

        public string RoomBed { get; set; }


        public string CheckIn { get; set; }

        public string CheckOut { get; set; }


        public List<string> Services { get; set; }


        public decimal TotalCost { get; set; }
    }
}