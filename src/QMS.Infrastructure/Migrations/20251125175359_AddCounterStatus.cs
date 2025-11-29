using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCounterStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Counters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Counters");
        }
    }
}
