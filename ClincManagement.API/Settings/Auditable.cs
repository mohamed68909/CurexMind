using System.ComponentModel.DataAnnotations.Schema;

namespace ClincManagement.API.Settings
{
    public abstract class Auditable
    {
        public bool IsDeleted { get; set; }
        public string CreatedById { get; set; } = default!;
        public string? UpdatedById { get; set; }
        public string? DeletedById { get; set; }

        public DateTime CreatedOn { get; set; } 
        public DateTime? UpdatedOn { get; set; }
        public DateTime? DeletedOn { get; set; }
        [NotMapped]
        public ApplicationUser CreatedBy { get; set; } = default!;
        public ApplicationUser? UpdatedBy { get; set; }
        [NotMapped]
        public ApplicationUser? DeletedBy { get; set; }




    }
}
