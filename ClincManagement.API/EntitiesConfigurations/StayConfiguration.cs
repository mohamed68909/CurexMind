
namespace ClincManagement.API.EntitiesConfigurations
{
    public class StayConfiguration : IEntityTypeConfiguration<Stay>

    {
        public void Configure(EntityTypeBuilder<Stay> builder)
        {

            builder.Property(s => s.CheckInDate)
                .IsRequired();
            builder.Property(s => s.CheckOutDate)
                .IsRequired();
            builder.Property(s => s.RoomNumber)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(s => s.BedNumber).
                IsRequired()
                .HasMaxLength(10);
            builder.Property(s => s.Notes).HasColumnType("nvarchar(max)")
                .IsRequired(false);
            builder.Property(s => s.Services)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);
            builder.Property(s => s.TotalCost).HasConversion<decimal>()
                .IsRequired()
                .HasColumnType("decimal(18,2)");
            builder.HasOne(s => s.Patient)
                .WithMany(p => p.Stays)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasIndex(s => s.CheckInDate)
                .HasDatabaseName("IX_StayCheckInDate");
            builder.HasIndex(s => s.RoomNumber).HasFilter("(IsActive = 1)")
                .HasDatabaseName("IX_StayRoomNumber");

            builder.HasIndex(s => s.IsActive)
                .HasDatabaseName("IX_StayIsActive");


        }
    }
}
