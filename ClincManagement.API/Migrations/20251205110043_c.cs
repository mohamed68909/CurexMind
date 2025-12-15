using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClincManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class c : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333001"));

        
            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "ClinicId",
                value: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "ClinicId",
                value: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "ClinicId",
                value: new Guid("33333333-3333-3333-3333-333333333333"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clinics",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.InsertData(
                table: "Clinics",
                columns: new[] { "Id", "CreatedDate", "Description", "IsActive", "Location", "Name" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333001"), new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "This is the main clinic located in the city center.", true, "123 Main St, Cityville", "Main Clinic" });

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "ClinicId",
                value: new Guid("33333333-3333-3333-3333-333333333001"));

            migrationBuilder.UpdateData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "ClinicId",
                value: new Guid("33333333-3333-3333-3333-333333333001"));

            migrationBuilder.UpdateData(
                table: "Reviews",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"),
                column: "ClinicId",
                value: new Guid("33333333-3333-3333-3333-333333333001"));
        }
    }
}
