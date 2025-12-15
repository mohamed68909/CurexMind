using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClincManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class i : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvoiceId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_InvoiceId",
                table: "Appointments",
                column: "InvoiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Invoices_InvoiceId",
                table: "Appointments",
                column: "InvoiceId",
                principalTable: "Invoices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Invoices_InvoiceId",
                table: "Appointments");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_InvoiceId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "InvoiceId",
                table: "Appointments");
        }
    }
}
