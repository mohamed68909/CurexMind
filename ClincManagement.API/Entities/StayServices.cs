namespace ClincManagement.API.Entities
{
    public class StayServices
    {
        public Guid StayId { get; set; }
        public Stay Stay { get; set; } = default!;

        public Guid ServiceId { get; set; }
        public MedicalService Service { get; set; } = default!;
    }

}
