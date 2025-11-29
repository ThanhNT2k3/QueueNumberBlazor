using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterServiceTypeMapping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_StaffId",
                table: "Tickets");

            migrationBuilder.CreateTable(
                name: "CounterServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CounterId = table.Column<int>(type: "INTEGER", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
                    IsPrimary = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterServiceTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CounterServiceTypes_Counters_CounterId",
                        column: x => x.CounterId,
                        principalTable: "Counters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CounterServiceTypes_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CounterServiceTypes_CounterId_ServiceTypeId",
                table: "CounterServiceTypes",
                columns: new[] { "CounterId", "ServiceTypeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CounterServiceTypes_ServiceTypeId",
                table: "CounterServiceTypes",
                column: "ServiceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_StaffId",
                table: "Tickets",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_StaffId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "CounterServiceTypes");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_StaffId",
                table: "Tickets",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
