using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClincManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class addpayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "AppointmentId", "ConfirmedAt", "CreatedAt", "InvoiceId", "Method", "PatientId", "Status", "TransactionId" },
                values: new object[] { new Guid("11111111-1111-1111-1111-111111111111"), 250.00m, new Guid("ef87e6b2-27b3-4a69-a28b-90064712980f"), new DateTime(2025, 12, 15, 10, 5, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 12, 15, 10, 0, 0, 0, DateTimeKind.Unspecified), new Guid("55555555-5555-5555-5555-555555555555"), 3, new Guid("11111111-1111-1111-1111-111111111111"), "Success", "TXN123456789" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));
        }
    }
}
