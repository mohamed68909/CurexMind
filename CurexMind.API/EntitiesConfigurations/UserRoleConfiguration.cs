using CurexMind.API.Abstractions.Consts;

namespace CurexMind.API.EntitiesConfigurations
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
                },
                new IdentityUserRole<string>
                {
                    RoleId = DefaultRoles.Patient.Id,
                    UserId = DefaultUsers.PatientUser.Id
                }
            );
        }
    }

}
