

namespace ClincManagement.API.EntitiesConfigurations
{
    public class PatientConfuguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.Property(n => n.NationalId)
                .IsRequired()
                .HasMaxLength(15);
           
            builder.HasKey(n => n.PatientId);


            builder.HasOne(n => n.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(n => n.UserId);

           

            builder.HasIndex(n => n.NationalId)
                .IsUnique();

        }
    }
}
