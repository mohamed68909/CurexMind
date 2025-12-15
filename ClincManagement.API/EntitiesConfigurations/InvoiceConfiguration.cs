using Microsoft.AspNetCore.Http.HttpResults;

namespace ClinicManagement.API.EntitiesConfigurations
{
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            builder.ToTable("Invoices");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.InvoiceNumber)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(i => i.InvoiceDate).IsRequired();
            builder.Property(i => i.DueDate).IsRequired(false);
            builder.Property(i => i.VisitDate).IsRequired();
            builder.Property(i => i.VisitTime).HasMaxLength(10).IsRequired();

            builder.Property(i => i.Notes)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(i => i.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(i => i.PaymentMethod)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(i => i.TotalAmountEGP)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.DiscountEGP)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(i => i.FinalAmountEGP)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(i => i.PaidAmountEGP)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.HasOne(i => i.Patient)
                .WithMany(p => p.Invoices)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
            




            builder.HasOne(i => i.ServiceType)
                .WithMany(s => s.Invoices)
                .HasForeignKey(i => i.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Payment)
                .WithOne(p => p.Invoice)
                .HasForeignKey<Payment>(p => p.InvoiceId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(i => i.InvoiceDate).HasDatabaseName("IX_Invoice_InvoiceDate");
            builder.HasIndex(i => i.Status).HasDatabaseName("IX_Invoice_Status");
            builder.HasIndex(i => i.PatientId).HasDatabaseName("IX_Invoice_PatientId");
            builder.HasIndex(i => i.DoctorId).HasDatabaseName("IX_Invoice_DoctorId");
            builder.HasIndex(i => i.ServiceTypeId).HasDatabaseName("IX_Invoice_ServiceTypeId");

            builder.HasData(
                new
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    DoctorId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    ClinicId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    ServiceTypeId = Guid.Parse("44444444-4444-4444-4444-444444444444"),

                    TotalAmountEGP = 500.00m,
                    DiscountEGP = 50.00m,
                    FinalAmountEGP = 450.00m,
                    PaidAmountEGP = 450.00m,

                    InvoiceNumber = "INV-00001",
                    InvoiceDate = new DateTime(2025, 09, 15),
                    DueDate = new DateTime(2025, 10, 15),
                    VisitDate = new DateTime(2025, 09, 10),
                    VisitTime = new TimeOnly(10, 0),
                    Status = InvoiceStatus.Paid,
                    PaymentMethod = "Cash",

                    Notes = "Initial consultation fee (includes 10% discount).",

                    IsDeleted = false,
                    CreatedDate = new DateTime(2025, 09, 15),
                    CreatedById = "System",
                    CreatedOn = new DateTime(2025, 09, 15),
                 

                }
            );
        }
    }
}