namespace ClincManagement.API.Entities
{

    public class MedicalService
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; } = decimal.Zero;
        public string? Description { get; set; }

        public ICollection<Stay>? Stays { get; set; }
    }
}



