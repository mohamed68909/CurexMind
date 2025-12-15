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

            
            builder.HasOne(p => p.Appointment)
                   .WithOne(a => a.Payment)
                   .HasForeignKey<Payment>(p => p.AppointmentId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.Invoice)
                   .WithOne(i => i.Payment)
                   .HasForeignKey<Payment>(p => p.InvoiceId)
                   .OnDelete(DeleteBehavior.Restrict);

           
            builder.HasIndex(p => p.CreatedAt)
                   .HasDatabaseName("IX_Payment_CreatedAt");

            builder.HasIndex(p => p.Status)
                   .HasDatabaseName("IX_Payment_Status");


            builder.HasData(
       new Payment
       {
           Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
           AppointmentId = Guid.Parse("ef87e6b2-27b3-4a69-a28b-90064712980f"),
           PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
           InvoiceId = Guid.Parse("55555555-5555-5555-5555-555555555555"),
           Amount = 250.00m,
           Method = PaymentMethod.Wallet,
           Status = PaymentStatus.Success,
           TransactionId = "TXN123456789",
           CreatedAt = new DateTime(2025, 12, 15, 10, 0, 0),  // قيمة ثابتة
           ConfirmedAt = new DateTime(2025, 12, 15, 10, 5, 0) // قيمة ثابتة
       }
   );


        }
    }
}
