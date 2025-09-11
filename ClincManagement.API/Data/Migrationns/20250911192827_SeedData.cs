using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ClincManagement.API.Data.Migrationns
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4E14506C-D3C0-4AE3-8616-5EB95A764358",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "admin-concurrency-stamp", "admin-security-stamp" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "IsDisabled", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "user-1", 0, "Cairo, Egypt", "doctor1-concurrency-stamp", "doctor1@example.com", true, "Doctor One", false, false, null, "DOCTOR1@EXAMPLE.COM", "DOCTOR1@EXAMPLE.COM", "AQAAAAIAAYagAAAAENCRYPTED_PASSWORD_HASH", null, false, "doctor1-security-stamp", false, "doctor1@example.com" },
                    { "user-2", 0, "Cairo, Egypt", "doctor2-concurrency-stamp", "doctor2@example.com", true, "Doctor Two", false, false, null, "DOCTOR2@EXAMPLE.COM", "DOCTOR2@EXAMPLE.COM", "AQAAAAIAAYagAAAAENCRYPTED_PASSWORD_HASH", null, false, "doctor2-security-stamp", false, "doctor2@example.com" },
                    { "user-3", 0, "Cairo, Egypt", "patient1-concurrency-stamp", "patient1@example.com", true, "Patient One", false, false, null, "PATIENT1@EXAMPLE.COM", "PATIENT1@EXAMPLE.COM", "AQAAAAIAAYagAAAAENCRYPTED_PASSWORD_HASH", null, false, "patient1-security-stamp", false, "patient1@example.com" }
                });

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "Location", "Name" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "General healthcare and diagnostics", true, "Cairo", "Future Clinic" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Specialized in dental care", true, "Alexandria", "Smile Dental Center" }
                });

            migrationBuilder.InsertData(
                table: "Doctors",
                columns: new[] { "Id", "ClinicId", "FullName", "Specialization", "userId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), new Guid("11111111-1111-1111-1111-111111111111"), "Dr. Ahmed Ali", "Cardiology", "user-1" },
                    { new Guid("44444444-4444-4444-4444-444444444444"), new Guid("22222222-2222-2222-2222-222222222222"), "Dr. Sara Hassan", "Dentistry", "user-2" }
                });

            migrationBuilder.InsertData(
                table: "Patients",
                columns: new[] { "PatientId", "CreatedDate", "DateOfBirth", "Gender", "IsActive", "NationalId", "Notes", "ProfileImageUrl", "SocialStatus", "UpdatedDate", "UserId" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1998, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, true, "29812345678901", "Heart condition", "", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "user-3" });

            migrationBuilder.InsertData(
                table: "Reviews",
                columns: new[] { "Id", "ClinicId", "Comment", "CreatedAt", "DoctorId", "PatientId", "Rating" },
                values: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), new Guid("11111111-1111-1111-1111-111111111111"), "Excellent doctor!", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new Guid("33333333-3333-3333-3333-333333333333"), new Guid("55555555-5555-5555-5555-555555555555"), 5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-2");

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-1");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "user-3");

            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "4E14506C-D3C0-4AE3-8616-5EB95A764358",
                columns: new[] { "ConcurrencyStamp", "SecurityStamp" },
                values: new object[] { "CE9E600E-ECD5-4400-92E6-986F63EEC953", "2FCB053BC1F041F2B07D3E7608D8020E" });
        }
    }
}
