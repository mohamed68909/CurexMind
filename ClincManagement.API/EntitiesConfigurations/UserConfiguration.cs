using ClincManagement.API.Abstractions.Consts;

namespace ClincManagement.API.EntitiesConfigurations
{
    public class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(100);

            builder.OwnsOne(u => u.ProfileImageUrl, img =>
            {
                img.ToTable("UserProfileImages");
                img.WithOwner().HasForeignKey("UserId");
                img.Property(x => x.FileName).HasMaxLength(250);
                img.Property(x => x.StoredFileName).HasMaxLength(250);
                img.Property(x => x.FileExtension).HasMaxLength(10);
                img.Property(x => x.ContentType).HasMaxLength(50);
            });

            builder.HasData(
                new ApplicationUser
                {
                    Id = DefaultUsers.Admin.Id,
                    FullName = $"{DefaultUsers.Admin.FirstName} {DefaultUsers.Admin.LastName}",
                    Email = DefaultUsers.Admin.Email,
                    NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
                    UserName = DefaultUsers.Admin.Email,
                    NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
                    PasswordHash = DefaultUsers.Admin.Password, // تأكد أنها hash
                    EmailConfirmed = true,
                    SecurityStamp = "admin-security-stamp",
                    ConcurrencyStamp = "admin-concurrency-stamp",
                    PhoneNumber = "01234567890",
                    PhoneNumberConfirmed = true
                }
            );
        }
    }
}
