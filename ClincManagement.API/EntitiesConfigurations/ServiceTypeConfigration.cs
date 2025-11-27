//using ClincManagement.API.Entities;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;

//namespace ClincManagement.API.EntitiesConfigurations
//{
//    public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
//    {
//        public void Configure(EntityTypeBuilder<ServiceType> builder)
//        {
//            builder.ToTable("ServiceTypes");

//            builder.HasKey(st => st.Id);

//            builder.Property(st => st.Name)
//                .IsRequired()
//                .HasMaxLength(100);

//            builder.Property(st => st.Description)
//                .HasMaxLength(500);

//            builder.Property(st => st.IsActive)
//                .IsRequired();

//            builder.Property(st => st.DefaultPriceEGP)
//                .HasColumnType("decimal(18,2)")
//                .IsRequired();

//            builder.HasData(
//                new ServiceType
//                {
//                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
//                    Name = "Consultation",
//                    Description = "General medical consultation service.",
//                    DefaultPriceEGP = 500.00m,
//                    IsActive = true,
//                    CreatedById = "System",
//                    CreatedOn = new DateTime(2025, 09, 01),
//                    IsDeleted = false
//                }
//            );
//        }
//    }
//}
