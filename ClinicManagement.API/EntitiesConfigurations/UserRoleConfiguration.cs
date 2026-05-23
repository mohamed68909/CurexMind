using ClinicManagement.API.Abstractions.Consts;

namespace ClinicManagement.API.EntitiesConfigurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = DefaultRoles.Admin.Id,
                    UserId = DefaultUsers.Admin.Id
                }
            );
        }
    }

}
