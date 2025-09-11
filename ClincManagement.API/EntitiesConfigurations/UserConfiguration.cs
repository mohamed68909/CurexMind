using ClincManagement.API.Abstractions.Consts;
using Microsoft.EntityFrameworkCore;
using System.Runtime.Intrinsics.X86;

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


            builder.HasData(
                new ApplicationUser
                {
                    Id = DefaultUsers.Admin.Id,
                    FullName = $"{DefaultUsers.Admin.FirstName} {DefaultUsers.Admin.LastName}",
                    Email = DefaultUsers.Admin.Email,
                    NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
                    UserName = DefaultUsers.Admin.Email,
                    NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
                    PasswordHash = DefaultUsers.Admin.Password,
                    EmailConfirmed = true,
                    SecurityStamp = "admin-security-stamp",
                    ConcurrencyStamp = "admin-concurrency-stamp",
                    Address = "Cairo, Egypt",
                    PhoneNumber = "01234567890",
                    PhoneNumberConfirmed = true
                },

                new ApplicationUser
                {
                    Id = "user-1",
                    UserName = "doctor1@example.com",
                    FullName = "Doctor One",
                    NormalizedUserName = "DOCTOR1@EXAMPLE.COM",
                    Email = "doctor1@example.com",
                    NormalizedEmail = "DOCTOR1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAENCRYPTED_PASSWORD_HASH",
                    SecurityStamp = "doctor1-security-stamp",
                    ConcurrencyStamp = "doctor1-concurrency-stamp",
                    Address = "Cairo, Egypt"
                },

                new ApplicationUser
                {
                    Id = "user-2",
                    UserName = "doctor2@example.com",
                    FullName = "Doctor Two",
                    NormalizedUserName = "DOCTOR2@EXAMPLE.COM",
                    Email = "doctor2@example.com",
                    NormalizedEmail = "DOCTOR2@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAENCRYPTED_PASSWORD_HASH",
                    SecurityStamp = "doctor2-security-stamp",
                    ConcurrencyStamp = "doctor2-concurrency-stamp",
                    Address = "Cairo, Egypt"
                },

                new ApplicationUser
                {
                    Id = "user-3",
                    UserName = "patient1@example.com",
                    NormalizedUserName = "PATIENT1@EXAMPLE.COM",
                    FullName = "Patient One",
                    Email = "patient1@example.com",
                    NormalizedEmail = "PATIENT1@EXAMPLE.COM",
                    EmailConfirmed = true,
                    PasswordHash = "AQAAAAIAAYagAAAAENCRYPTED_PASSWORD_HASH",
                    SecurityStamp = "patient1-security-stamp",
                    ConcurrencyStamp = "patient1-concurrency-stamp",
                    Address = "Cairo, Egypt"
                }
            );






        }
    }
}
