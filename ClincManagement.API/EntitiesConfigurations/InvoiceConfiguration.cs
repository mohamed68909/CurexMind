namespace ClincManagement.API.EntitiesConfigurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.Property(i => i.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Status)
                .IsRequired();

            builder.Property(i => i.DueDate)
                .IsRequired();

            builder.Property(i => i.CreatedDate)
                .HasDefaultValueSql("getdate()")
                .IsRequired();

            builder.Property(i => i.InvoiceDate)
                .IsRequired();

            builder.Property(i => i.PaidAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.RemainingAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.Notes)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.HasOne(i => i.Patient)
                .WithMany(p => p.Invoice)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.NoAction); 

            builder.HasIndex(i => i.InvoiceDate)
                .HasDatabaseName("IX_InvoiceDate");

            builder.HasIndex(i => i.Status)
                .HasDatabaseName("IX_InvoiceStatus");

            builder.HasIndex(i => i.PatientId)
                .HasDatabaseName("IX_InvoicePatientId");
        }
    }
}
