using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class ServiceType : Auditable
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal DefaultPriceEGP { get; set; }

        public bool IsActive { get; set; } = true;

        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
