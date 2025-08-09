namespace ClincManagement.API.Entities
{
    public sealed class Doctor
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string Specialization { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;



        public ApplicationUser User { get; set; } = default!;


    }
}
