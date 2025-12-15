using ClincManagement.API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClincManagement.API.EntitiesConfigurations
{
    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
    {
        public void Configure(EntityTypeBuilder<ServiceType> builder)
        {
            builder.ToTable("ServiceTypes");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Seed Data
            builder.HasData(
                new ServiceType
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Consultation"
                },
                new ServiceType
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Installation"
                },
                new ServiceType
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    Name = "Maintenance"
                },
                new ServiceType
                {
                    Id = Guid.Parse("77777777-7777-7777-7777-777777777777"),
                    Name = "Emergency Repair"
                }
            );
        }
    }
}