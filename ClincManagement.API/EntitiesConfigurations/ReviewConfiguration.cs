using Microsoft.EntityFrameworkCore;

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
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(r => r.Doctor)
                   .WithMany(d => d.Reviews)
                   .HasForeignKey(r => r.DoctorId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder
        .HasOne(r => r.User)
        .WithMany()
        .HasForeignKey(r => r.UserId)
        .OnDelete(DeleteBehavior.Restrict);


            builder.HasOne(r => r.Clinic)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.ClinicId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasData(
                new Review
                {
                    Id = Guid.Parse("88888888-8888-8888-8888-888888888888"),
                    Rating = 5,
                    Comment = "Excellent service!",
                    CreatedAt = new DateTime(2025, 09, 15),
                    ClinicId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    UserId = "f70250f2-ece4-44da-a1a8-ffad173d3dde",
                    DoctorId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                }
            );
        }
    }
}
