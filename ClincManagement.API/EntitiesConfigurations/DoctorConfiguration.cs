namespace ClincManagement.API.EntityConfiguration
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(d => d.Id);

          

            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.YearsOfExperience)
                .IsRequired();

            builder.Property(d => d.Languages)
                .HasMaxLength(200);

          
            builder.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.NoAction);
        

           



        builder.HasOne(d => d.Clinic)
                .WithMany(c => c.Doctors)
                .HasForeignKey(d => d.ClinicId)
                .OnDelete(DeleteBehavior.NoAction);

         
       

         
            builder.HasData(
                new Doctor
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    FullName = "Dr. John Smith",
                    Specialization = "Cardiology",
                    YearsOfExperience = 12,
                    ClinicId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    UserId = "4E14506C-D3C0-4AE3-8616-5EB95A764358",
                    Languages = "English, Spanish",
                    CreatedById = "system"
                }
            );
        }
    }
}
