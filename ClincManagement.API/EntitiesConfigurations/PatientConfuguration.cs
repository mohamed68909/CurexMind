namespace ClincManagement.API.EntitiesConfigurations
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            builder.HasKey(n => n.PatientId);

            builder.Property(n => n.NationalId)
                .IsRequired()
                .HasMaxLength(15);

            builder.Property(u => u.Address)
                .IsRequired()
                .HasMaxLength(300);

            builder.HasOne(n => n.User)
                .WithOne(u => u.Patient)
                .HasForeignKey<Patient>(n => n.UserId)
                .OnDelete(DeleteBehavior.NoAction);
     
            builder.HasIndex(n => n.NationalId)
                .IsUnique();

            builder.HasData(
                new Patient
                {
                    PatientId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    NationalId = "29812345678901",
                    Gender = Gender.Male,
                    Notes = "First patient note",
                    DateOfBirth = new DateTime(1998, 5, 20),
                    CreatedDate = new DateTime(2025, 09, 15),
                    UpdatedDate = new DateTime(2025, 09, 15),
                    SocialStatus = SocialStatus.Single,
                    UserId = "4E14506C-D3C0-4AE3-8616-5EB95A764358",
                    CreatedById = "system"
                }
            );
        }
    }
}
