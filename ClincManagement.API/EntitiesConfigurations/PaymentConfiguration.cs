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


    


        }
    }
}
