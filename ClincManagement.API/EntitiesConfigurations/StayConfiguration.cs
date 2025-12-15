namespace ClincManagement.API.EntitiesConfigurations
{
    public class StayConfiguration : IEntityTypeConfiguration<Stay>
    {
        public void Configure(EntityTypeBuilder<Stay> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.PatientId).IsRequired();

            builder.Property(s => s.RoomNumber)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.BedNumber)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(s => s.Department)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.Status)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.StayType)
                .HasConversion<string>()
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(s => s.StartDate)
                .IsRequired();

            builder.Property(s => s.EndDate)
                .IsRequired(false);

            builder.Property(s => s.Notes)
                .HasColumnType("nvarchar(max)")
                .IsRequired(false);

            builder.HasOne(s => s.Patient)
                .WithMany(p => p.Stays)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(s => s.RoomNumber)
                .HasDatabaseName("IX_StayRoomNumber");

           
            builder.HasData(
                new Stay
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),

                    Department = "General Medicine",
                    RoomNumber = "101A",
                    BedNumber = "B1",

                    StayType = StayType.Inpatient,
                    Status = StayStatus.Active,

                    StartDate = new DateTime(2025, 09, 15),
                    EndDate = null,

                    Notes = "Patient admitted for observation."
                }
            );
        }
    }
}
