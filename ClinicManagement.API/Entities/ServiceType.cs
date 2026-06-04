namespace ClinicManagement.API.Entities
{
    public class ServiceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

      
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }

}
