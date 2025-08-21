namespace ClincManagement.API.EntityCognfigfigui
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.Address)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.Email).IsRequired()
                .HasMaxLength(100);
            builder.Property(u => u.IsActive).IsRequired()
                .HasDefaultValue(true);
            builder.Property(u => u.CreatedDate).HasConversion(
                v => v,
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc))
                .HasDefaultValueSql("getutcdate()");




        }
    }
}
