namespace ClincManagement.API.EntityCognfigfigui
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);


            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(200);
           

            builder.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.userId);
            builder.Property(d => d.YearsOfExperience).IsRequired();
            



            builder.HasData(
    new Doctor
    {
        Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
        Specialization = "Cardiology",
        YearsOfExperience = 12,
        ClinicId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
        FullName = "Dr. John Smith",
        userId = "4E14506C-D3C0-4AE3-8616-5EB95A764358",
        languages = ["English", "Spanish"],

    }
);

        }
    }


    }

