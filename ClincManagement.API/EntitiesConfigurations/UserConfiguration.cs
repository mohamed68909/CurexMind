namespace ClincManagement.API.EntityCognfigfigui
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

        }
    }
}
