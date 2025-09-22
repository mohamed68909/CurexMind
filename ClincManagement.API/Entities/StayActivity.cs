using ClincManagement.API.Settings;

namespace ClincManagement.API.Entities
{
    public class StayActivity : Auditable
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid StayId { get; set; }
        public string Action { get; set; } = string.Empty;
        public string ByUser { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Stay Stay { get; set; } = default!;
    }
}
