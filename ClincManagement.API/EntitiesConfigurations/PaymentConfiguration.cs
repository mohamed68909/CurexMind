namespace ClincManagement.API.EntitiesConfigurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            // Primary Key
            builder.HasKey(p => p.Id);

            // Properties
            builder.Property(p => p.Amount)
                   .IsRequired()
                   .HasColumnType("decimal(18,2)");

            builder.Property(p => p.Method)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(p => p.Status)
                   .IsRequired()
                   .HasConversion<string>()  // Store enum as string
                   .HasMaxLength(20);

            builder.Property(p => p.TransactionId)
                   .HasMaxLength(500)
                   .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                   .IsRequired()
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(p => p.ConfirmedAt)
                   .IsRequired(false);

            // 🔗 Relationships
            builder.HasOne(p => p.Appointment)
                   .WithOne(a => a.Payment)
                   .HasForeignKey<Payment>(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Invoice)
                   .WithOne(i => i.Payment)
                   .HasForeignKey<Payment>(p => p.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

            // ✅ Indexes
            builder.HasIndex(p => p.CreatedAt)
                   .HasDatabaseName("IX_Payment_CreatedAt");

            builder.HasIndex(p => p.Status)
                   .HasDatabaseName("IX_Payment_Status");

            // 🧩 Seed Data
            builder.HasData(
                new Payment
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555556"),
                    AppointmentId = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    InvoiceId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Amount = 200.00m,

                    Status = PaymentStatus.Success,
                    TransactionId = "TRX123456789",
                    CreatedAt = new DateTime(2025, 09, 10),
                    ConfirmedAt = new DateTime(2025, 09, 10)
                }
            );
        }
    }
}
