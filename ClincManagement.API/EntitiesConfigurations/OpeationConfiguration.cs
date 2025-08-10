namespace ClincManagement.API.EntitiesConfigurations
{
    public class OperationConfiguration : IEntityTypeConfiguration<Operation>
    {
        public void Configure(EntityTypeBuilder<Operation> builder)
        {
            builder.Property(o => o.Date)
                .IsRequired();

            builder.Property(o => o.Notes)
                .IsRequired(false)
                .HasColumnType("nvarchar(max)");

            builder.Property(o => o.Tools)
                .HasColumnType("nvarchar(max)");

            builder.Property(o => o.Cost)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.HasOne(o => o.Patient)
                .WithMany(p => p.Operations)
                .HasForeignKey(o => o.PatientId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.HasOne(o => o.Doctor)
                .WithMany(d => d.Operations)
                .HasForeignKey(o => o.SurgeonId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.HasIndex(o => o.Date)
                .HasDatabaseName("IX_OperationDate");

            builder.HasIndex(o => o.PatientId)
                .HasDatabaseName("IX_OperationPatientId");

            builder.HasIndex(o => o.SurgeonId)
                .HasDatabaseName("IX_OperationSurgeonId");
        }
    }
}
