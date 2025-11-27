namespace ClincManagement.API.EntitiesConfigurations
{
    public class StayConfiguration : IEntityTypeConfiguration<Stay>
    {
        public void Configure(EntityTypeBuilder<Stay> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.PatientId).IsRequired();
            builder.Property(s => s.RoomNumber).IsRequired().HasMaxLength(20);
            builder.Property(s => s.BedNumber).IsRequired().HasMaxLength(10);
            builder.Property(s => s.Department).IsRequired().HasMaxLength(50);
            builder.Property(s => s.Status).HasConversion<string>().IsRequired().HasMaxLength(50);
           // builder.Property(s => s.CheckInDate).IsRequired();
            //builder.Property(s => s.CheckOutDate).IsRequired(false);
            builder.Property(s => s.Notes).HasColumnType("nvarchar(max)").IsRequired(false);
           
           // builder.Property(s => s.TotalCost).IsRequired().HasColumnType("decimal(18,2)");

            builder.HasOne(s => s.Patient)
                .WithMany(p => p.Stays)
                .HasForeignKey(s => s.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

           // builder.HasIndex(s => s.CheckInDate).HasDatabaseName("IX_StayCheckInDate");
            builder.HasIndex(s => s.RoomNumber).HasDatabaseName("IX_StayRoomNumber");

            builder.HasData(
      new Stay
      {
          Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
          PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),

         // ServiceTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444"), 

          Department = "General Medicine",
          RoomNumber = "101A",
          BedNumber = "B1",
          //CheckInDate = new DateTime(2025, 09, 15),
          //CheckOutDate = new DateTime(1900, 1, 1),

         // TotalCost = 1500m,
          Notes = "Patient admitted for observation.",
         // CreatedById = "System"
      }
  );

        }
    }
}
