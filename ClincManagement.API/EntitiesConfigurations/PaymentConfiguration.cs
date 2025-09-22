// Corrected PaymentConfiguration.cs

using ClincManagement.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClincManagement.API.EntitiesConfigurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

           
            builder.Property(p => p.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("getdate()");

            builder.Property(p => p.Method)
                .IsRequired()
                .HasMaxLength(50);

          
            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

           
            builder.Property(p => p.TransactionId)
                .HasMaxLength(500)
                .IsRequired(false);


            

            
            builder.HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_PaymentCreatedAt");

            builder.HasIndex(p => p.Status)
                .HasDatabaseName("IX_PaymentStatus");

            // Seed Payment with corrected values
            builder.HasData(
                new Payment
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555556"),
                    AppointmentId = Guid.Parse("44444444-4444-4444-4444-444444444444"), 
                    InvoiceId = Guid.Parse("55555555-5555-5555-5555-555555555555"), 
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"), 
                    Amount = 200.00m,
                    CreatedAt = new DateTime(2025, 09, 10),
                    Method = "Credit Card", 
                    Status = PaymentStatus.Success, 
                    TransactionId = "TRX123456789",
                    ConfirmedAt = new DateTime(2025, 09, 10)
                }
            );
        }
    }
}
