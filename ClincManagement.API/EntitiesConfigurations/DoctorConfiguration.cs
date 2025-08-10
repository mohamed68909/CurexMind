namespace ClincManagement.API.EntityCognfigfigui
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {


            builder.Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasOne(d => d.User)
                .WithOne(u => u.Doctor)
                .HasForeignKey<Doctor>(d => d.userId);
            
        }
    }
}
