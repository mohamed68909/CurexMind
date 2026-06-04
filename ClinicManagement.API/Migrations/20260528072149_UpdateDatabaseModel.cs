using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDatabaseModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "bio",
                table: "Doctors",
                newName: "Bio");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Bio",
                table: "Doctors",
                newName: "bio");
        }
    }
}
