using ClincManagement.API.Abstractions.Consts;

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
           
            

            builder.HasData([
                new ApplicationUser
            {
                      Id = DefaultUsers.Admin.Id,
                     FullName = $"{DefaultUsers.Admin.FirstName} {DefaultUsers.Admin.LastName}",
                      Email = DefaultUsers.Admin.Email,
                      NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
                      UserName = DefaultUsers.Admin.Email,
                      NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
                      PasswordHash =  DefaultUsers.Admin.Password,
                      EmailConfirmed = true,
                      SecurityStamp  = DefaultUsers.Admin.SecurityStamp,
                      ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
                     Address = "Cairo, Egypt",
                     PhoneNumber = "01234567890",
                      PhoneNumberConfirmed = true
            
            }
                ]);


        }
    }
}
