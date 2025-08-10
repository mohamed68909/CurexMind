namespace ClincManagement.API.Entities
{
    public class Operation
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid PatientId { get; set; }
        public Guid SurgeonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Tools { get; set; }  = string .Empty;
        public decimal Cost { get; set; }
        public string? Notes {  get; set; }
        public Patient Patient { get; set; } = default!;
        public Doctor Doctor { get; set; } = default!;

    }
}
