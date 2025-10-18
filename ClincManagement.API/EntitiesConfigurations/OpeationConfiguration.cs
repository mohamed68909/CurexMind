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
            builder.HasData(
                new Operation
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    SurgeonId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Heart Surgery",
                    Date = new DateTime(2025, 10, 15),
                    Tools = "Scalpel, Monitor",
                    Cost = 20000,
                    Notes = "Critical operation"
                }
            );

        }
    }
}
