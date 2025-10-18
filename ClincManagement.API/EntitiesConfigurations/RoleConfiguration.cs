using ClincManagement.API.Abstractions.Consts;

namespace ClincManagement.API.EntitiesConfigurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData(
                new ApplicationRole
                {
                    Id = DefaultRoles.Admin.Id,
                    Name = DefaultRoles.Admin.Name,
                    NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp
                },
                new ApplicationRole
                {
                    Id = DefaultRoles.Patient.Id,
                    Name = DefaultRoles.Patient.Name,
                    NormalizedName = DefaultRoles.Patient.Name.ToUpper(),
                    ConcurrencyStamp = DefaultRoles.Patient.ConcurrencyStamp
                }
            );
        }
    }
}
