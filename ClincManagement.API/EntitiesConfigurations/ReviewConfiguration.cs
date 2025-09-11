
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
           
        }
    }
    }

