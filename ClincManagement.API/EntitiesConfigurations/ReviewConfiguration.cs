
namespace ClincManagement.API.EntitiesConfigurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
          
      
            builder.HasKey(r => r.Id);

        
            builder.Property(r => r.Rating)
                   .IsRequired()
                   .HasColumnType("int");

            builder.Property(r => r.Comment)
                   .HasMaxLength(1000);

            builder.Property(r => r.CreatedAt)
                   .HasDefaultValueSql("GETUTCDATE()");

          
            builder.HasOne(r => r.Doctor)
                   .WithMany(d => d.Reviews)
                   .HasForeignKey(r => r.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);
         
            builder.HasOne(r => r.Patient)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.PatientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasData(
                new Review
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Rating = 5,
                    Comment = "Excellent service!",
                    CreatedAt = new DateTime(2025, 09, 15, 10, 0, 0, DateTimeKind.Utc), // ثابتة
                    ClinicId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    DoctorId = Guid.Parse("44444444-4444-4444-4444-444444444444")
                }
            );


        }
    }
    }

