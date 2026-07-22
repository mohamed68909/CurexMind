using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CurexMind.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultPatientUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FullName", "IsDisabled", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "5F24506C-D3C0-4AE3-8616-5EB95A764359", 0, "DE9E600E-ECD5-4400-92E6-986F63EEC954", "Patient@mohamed.com", true, "Ali Hassan", false, false, null, "PATIENT@MOHAMED.COM", "PATIENT@MOHAMED.COM", "AQAAAAIAAYagAAAAEKj70KPmPc7BxyRhD9MuptCGolRkbmTp27lM/5HLVQxdU/qZw0HwYDAGR9JyB4c19Q==", "01234567891", true, "3FCB053BC1F041F2B07D3E7608D8020F", false, "Patient@mohamed.com" });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UserId",
                value: "5F24506C-D3C0-4AE3-8616-5EB95A764359");

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "4D447E8A-B35A-4DAE-BCE3-4552BF828693", "5F24506C-D3C0-4AE3-8616-5EB95A764359" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4D447E8A-B35A-4DAE-BCE3-4552BF828693", "5F24506C-D3C0-4AE3-8616-5EB95A764359" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "5F24506C-D3C0-4AE3-8616-5EB95A764359");

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "UserId",
                value: "4E14506C-D3C0-4AE3-8616-5EB95A764358");
        }
    }
}
