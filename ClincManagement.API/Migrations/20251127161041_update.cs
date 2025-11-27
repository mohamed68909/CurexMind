using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClincManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invoices_ServiceTypes_ServiceTypeId",
                table: "Invoices");

            migrationBuilder.DropForeignKey(
                name: "FK_Stays_AspNetUsers_UpdatedById",
                table: "Stays");

            migrationBuilder.DropForeignKey(
                name: "FK_Stays_ServiceTypes_ServiceTypeId",
                table: "Stays");

            migrationBuilder.DropTable(
                name: "MedicalServiceStay");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "StayActivities");

            migrationBuilder.DropTable(
                name: "MedicalServices");

            migrationBuilder.DropIndex(
                name: "IX_StayCheckInDate",
                table: "Stays");

            migrationBuilder.DropIndex(
                name: "IX_Stays_ServiceTypeId",
                table: "Stays");

            migrationBuilder.DropIndex(
                name: "IX_Stays_UpdatedById",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "CheckInDate",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "CheckOutDate",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "CreatedById",
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
                name: "ServiceTypeId",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Stays");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Stays");

            migrationBuilder.RenameColumn(
                name: "UpdatedOn",
                table: "Stays",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "CreatedOn",
                table: "Stays",
                newName: "StartDate");

            migrationBuilder.AddColumn<int>(
                name: "StayType",
                table: "Stays",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Stays",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "Status", "StayType" },
                values: new object[] { "Active", 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StayType",
                table: "Stays");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Stays",
                newName: "CreatedOn");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Stays",
                newName: "UpdatedOn");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInDate",
                table: "Stays",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckOutDate",
                table: "Stays",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "Stays",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

            migrationBuilder.AddColumn<Guid>(
                name: "ServiceTypeId",
                table: "Stays",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Stays",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedById",
                table: "Stays",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MedicalServices",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DefaultPriceEGP = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTypes_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StayActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UpdatedById = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedById = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedById = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StayActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StayActivities_AspNetUsers_UpdatedById",
                        column: x => x.UpdatedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StayActivities_Stays_StayId",
                        column: x => x.StayId,
                        principalTable: "Stays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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

            migrationBuilder.InsertData(
                table: "ServiceTypes",
                columns: new[] { "Id", "CreatedById", "CreatedOn", "DefaultPriceEGP", "DeletedById", "DeletedOn", "Description", "IsActive", "IsDeleted", "Name", "UpdatedById", "UpdatedOn" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "System", new DateTime(2025, 9, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 500.00m, null, null, "General medical consultation service.", true, false, "Consultation", null, null });

            migrationBuilder.UpdateData(
                table: "Stays",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "CheckInDate", "CheckOutDate", "CreatedById", "DeletedById", "DeletedOn", "IsDeleted", "ServiceTypeId", "Status", "TotalCost", "UpdatedById" },
                values: new object[] { new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(1900, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", null, null, false, new Guid("44444444-4444-4444-4444-444444444444"), "Admitted", 1500m, null });

            migrationBuilder.CreateIndex(
                name: "IX_StayCheckInDate",
                table: "Stays",
                column: "CheckInDate");

            migrationBuilder.CreateIndex(
                name: "IX_Stays_ServiceTypeId",
                table: "Stays",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Stays_UpdatedById",
                table: "Stays",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalServiceStay_StaysId",
                table: "MedicalServiceStay",
                column: "StaysId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTypes_UpdatedById",
                table: "ServiceTypes",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StayActivities_StayId",
                table: "StayActivities",
                column: "StayId");

            migrationBuilder.CreateIndex(
                name: "IX_StayActivities_UpdatedById",
                table: "StayActivities",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Invoices_ServiceTypes_ServiceTypeId",
                table: "Invoices",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Stays_AspNetUsers_UpdatedById",
                table: "Stays",
                column: "UpdatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Stays_ServiceTypes_ServiceTypeId",
                table: "Stays",
                column: "ServiceTypeId",
                principalTable: "ServiceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
