
namespace ClincManagement.API.EntitiesConfigurations
{
    public class VitalSignConfigurations : IEntityTypeConfiguration<VitalSigns>
    {
        public void Configure(EntityTypeBuilder<VitalSigns> builder)
        {

            builder.ToTable("VitalSigns");
            builder.HasKey(vs => vs.Id);
            builder.Property(vs => vs.Id)
                .ValueGeneratedOnAdd()
                .HasDefaultValueSql("NEWID()");
            builder.Property(vs => vs.RecordedDate)
                .HasDefaultValueSql("GETDATE()").IsRequired(false);


            builder.HasOne(vs => vs.Patient)
                .WithMany(p => p.VitalSigns)
                .HasForeignKey(vs => vs.PatientId)
                .OnDelete(DeleteBehavior.NoAction);
            
            builder.HasOne(vs => vs.User)
                .WithMany()
                .HasForeignKey(vs => vs.RecordedBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(vs => vs.BloodPressureSystolic).IsRequired(false);
            builder.Property(vs => vs.BloodPressureDiastolic).IsRequired(false);
            builder.Property(vs => vs.HeartRate).IsRequired(false);
            builder.Property(vs => vs.Temperature)
                .HasPrecision(5, 2).IsRequired(false);
            builder.Property(vs => vs.Weight).HasPrecision(5, 2).IsRequired(false);
            builder.HasIndex(vs => vs.RecordedDate)
                .HasDatabaseName("IX_VitalSigns_RecordedDate");
            builder.HasIndex(vs => vs.PatientId).SortInTempDb(true)
                .HasDatabaseName("IX_VitalSigns_PatientId");


        }
    }
}
