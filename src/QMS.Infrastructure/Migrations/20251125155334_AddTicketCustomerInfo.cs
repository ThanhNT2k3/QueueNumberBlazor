using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketCustomerInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerEmail",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerNotes",
                table: "Tickets",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "Tickets",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerEmail",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CustomerNotes",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "Tickets");
        }
    }
}
