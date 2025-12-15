namespace ClincManagement.API.Entities
{
    public class ServiceType
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

      
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }

}
