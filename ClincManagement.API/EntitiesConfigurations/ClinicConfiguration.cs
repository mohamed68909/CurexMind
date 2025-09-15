using Microsoft.EntityFrameworkCore;

namespace ClincManagement.API.EntitiesConfigurations
{
    public class ClinicConfiguration : IEntityTypeConfiguration<Clinic>
    {
        public void Configure(EntityTypeBuilder<Clinic> builder)
        {

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(c => c.Location)
                .IsRequired()
                .HasMaxLength(255);
            builder.Property(c => c.CreatedDate)
                .IsRequired().HasDefaultValueSql("getdate()");
            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);
            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasMaxLength(1);



            builder.HasMany(c => c.Appointments)
                .WithOne(a => a.Clinic)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);
           
            builder.HasIndex(c => c.Name)
                .IsUnique()
                .HasDatabaseName("IX_ClinicName");
            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_ClinicIsActive");


            builder.HasData(
      new Clinic
      {
          Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
          Name = "Main Clinic",
          Description = "This is the main clinic located in the city center.",
          Location = "123 Main St, Cityville",
          IsActive = true,
          CreatedDate = new DateTime(2025, 9, 15),

      }
   );











        }
    }
}
