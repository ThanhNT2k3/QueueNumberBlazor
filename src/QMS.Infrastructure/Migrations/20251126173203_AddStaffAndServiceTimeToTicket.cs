using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffAndServiceTimeToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndServiceTime",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Tickets",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartServiceTime",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CounterAssignmentHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CounterId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    AssignedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UnassignedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    AssignedBy = table.Column<string>(type: "TEXT", nullable: false),
                    UnassignedBy = table.Column<string>(type: "TEXT", nullable: true),
                    Notes = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CounterAssignmentHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CounterAssignmentHistories_Counters_CounterId",
                        column: x => x.CounterId,
                        principalTable: "Counters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CounterAssignmentHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_StaffId",
                table: "Tickets",
                column: "StaffId");

            migrationBuilder.CreateIndex(
                name: "IX_CounterAssignmentHistories_CounterId",
                table: "CounterAssignmentHistories",
                column: "CounterId");

            migrationBuilder.CreateIndex(
                name: "IX_CounterAssignmentHistories_UserId",
                table: "CounterAssignmentHistories",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Users_StaffId",
                table: "Tickets",
                column: "StaffId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Users_StaffId",
                table: "Tickets");

            migrationBuilder.DropTable(
                name: "CounterAssignmentHistories");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_StaffId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EndServiceTime",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "StartServiceTime",
                table: "Tickets");
        }
    }
}
