
namespace ClincManagement.API.EntitiesConfigurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {


            builder.Property(a => a.AppointmentDate)
                .IsRequired();
            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(20);
            builder.Property(a => a.Type)
                .IsRequired()
                .HasMaxLength(50);
            builder.Property(a => a.AppointmentTime).IsRequired();
            builder.Property(a => a.Notes)
                .HasMaxLength(500)
                .IsRequired(false);
            builder.Property(a => a.CreatedDate).HasDefaultValueSql("getdate()").IsRequired();
            builder.Property(a => a.UpdatedDate).HasDefaultValueSql("getdate()").IsRequired();
            builder.Property(a => a.Duration)
                .IsRequired()
                .HasDefaultValue(30); 

            builder.HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.NoAction)
                ;
            builder.HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.NoAction)
               ;
            builder.HasOne(a => a.Clinic)
                .WithMany(c => c.Appointments)
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);
                ;

            builder.HasIndex (d=>d.AppointmentDate)
                .HasDatabaseName("IX_AppointmentDate");
            builder.HasIndex(d => d.Status)
                .HasDatabaseName("IX_AppointmentStatus");



        }
    }
}
