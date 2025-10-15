using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClincManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class Addstay : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StayActivity_AspNetUsers_UpdatedById",
                table: "StayActivity");

            migrationBuilder.DropForeignKey(
                name: "FK_StayActivity_Stays_StayId",
                table: "StayActivity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StayActivity",
                table: "StayActivity");

            migrationBuilder.RenameTable(
                name: "StayActivity",
                newName: "StayActivities");

            migrationBuilder.RenameIndex(
                name: "IX_StayActivity_UpdatedById",
                table: "StayActivities",
                newName: "IX_StayActivities_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StayActivity_StayId",
                table: "StayActivities",
                newName: "IX_StayActivities_StayId");

            migrationBuilder.AddColumn<Guid>(
                name: "PatientId1",
                table: "Patients",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StayActivities",
                table: "StayActivities",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MedicalServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MedicalServiceStay",
                columns: table => new
                {
                    MedicalServicesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StaysId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalServiceStay", x => new { x.MedicalServicesId, x.StaysId });
                    table.ForeignKey(
                        name: "FK_MedicalServiceStay_MedicalServices_MedicalServicesId",
                        column: x => x.MedicalServicesId,
                        principalTable: "MedicalServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicalServiceStay_Stays_StaysId",
                        column: x => x.StaysId,
                        principalTable: "Stays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Patients",
                keyColumn: "PatientId",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "PatientId1",
                value: null);

            migrationBuilder.CreateIndex(
                name: "IX_Patients_PatientId1",
                table: "Patients",
                column: "PatientId1");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalServiceStay_StaysId",
                table: "MedicalServiceStay",
                column: "StaysId");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_Patients_PatientId1",
                table: "Patients",
                column: "PatientId1",
                principalTable: "Patients",
                principalColumn: "PatientId");

            migrationBuilder.AddForeignKey(
                name: "FK_StayActivities_AspNetUsers_UpdatedById",
                table: "StayActivities",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StayActivities_Stays_StayId",
                table: "StayActivities",
                column: "StayId",
                principalTable: "Stays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Patients_Patients_PatientId1",
                table: "Patients");

            migrationBuilder.DropForeignKey(
                name: "FK_StayActivities_AspNetUsers_UpdatedById",
                table: "StayActivities");

            migrationBuilder.DropForeignKey(
                name: "FK_StayActivities_Stays_StayId",
                table: "StayActivities");

            migrationBuilder.DropTable(
                name: "MedicalServiceStay");

            migrationBuilder.DropTable(
                name: "MedicalServices");

            migrationBuilder.DropIndex(
                name: "IX_Patients_PatientId1",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StayActivities",
                table: "StayActivities");

            migrationBuilder.DropColumn(
                name: "PatientId1",
                table: "Patients");

            migrationBuilder.RenameTable(
                name: "StayActivities",
                newName: "StayActivity");

            migrationBuilder.RenameIndex(
                name: "IX_StayActivities_UpdatedById",
                table: "StayActivity",
                newName: "IX_StayActivity_UpdatedById");

            migrationBuilder.RenameIndex(
                name: "IX_StayActivities_StayId",
                table: "StayActivity",
                newName: "IX_StayActivity_StayId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StayActivity",
                table: "StayActivity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StayActivity_AspNetUsers_UpdatedById",
                table: "StayActivity",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StayActivity_Stays_StayId",
                table: "StayActivity",
                column: "StayId",
                principalTable: "Stays",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
