namespace ClincManagement.API.EntitiesConfigurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.Property(a => a.AppointmentDate).IsRequired();
            builder.Property(a => a.Status).IsRequired().HasMaxLength(20);
            builder.Property(a => a.Type).IsRequired().HasMaxLength(50);
            builder.Property(a => a.AppointmentTime).IsRequired();
            builder.Property(a => a.Notes).HasMaxLength(500).IsRequired(false);
            builder.Property(a => a.UpdatedDate).IsRequired();
            builder.Property(a => a.Duration).IsRequired().HasDefaultValue(30);

            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);


            builder.HasOne(a => a.Payment)
                   .WithOne(p => p.Appointment)
                   .HasForeignKey<Payment>(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(d => d.AppointmentDate).HasDatabaseName("IX_AppointmentDate");
            builder.HasIndex(d => d.Status).HasDatabaseName("IX_AppointmentStatus");

            builder.HasData(
                new Appointment
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    DoctorId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // ✅ تعديل
                    ClinicId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    AppointmentDate = new DateTime(2025, 09, 18),
                    AppointmentTime = "10:00 AM",
                    Duration = 30,
                    Notes = "General check-up",
                    Type = AppointmentType.First_Visit,
                    Status = AppointmentStatus.Confirmed,
                    UpdatedDate = new DateTime(2025, 09, 15),
                    CreatedById = "system",
                    IsDeleted = false
                }
            );
        }
    }
}
