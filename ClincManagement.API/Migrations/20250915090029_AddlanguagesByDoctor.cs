using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClincManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddlanguagesByDoctor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "languages",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.UpdateData(
                table: "Doctors",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "languages",
                value: "[\"English\",\"Spanish\"]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "languages",
                table: "Doctors");
        }
    }
}
