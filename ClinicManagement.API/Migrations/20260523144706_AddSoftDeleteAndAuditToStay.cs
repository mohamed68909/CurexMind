using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteAndAuditToStay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Stays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Stays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedById",
                table: "Stays",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedOn",
                table: "Stays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stays",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Stays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedOn",
                table: "Stays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Stays",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CreatedById", "CreatedOn", "DeletedById", "DeletedOn", "IsDeleted", "UpdatedById", "UpdatedOn" },
                values: new object[] { "System", new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null, null, false, null, new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "IX_Stays_UpdatedById",
                table: "Stays",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Stays_AspNetUsers_UpdatedById",
                table: "Stays",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stays_AspNetUsers_UpdatedById",
                table: "Stays");

            migrationBuilder.DropIndex(
                name: "IX_Stays_UpdatedById",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "DeletedOn",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "UpdatedOn",
                table: "Stays");
        }
    }
}
