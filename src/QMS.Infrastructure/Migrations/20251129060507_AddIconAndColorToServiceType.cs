using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIconAndColorToServiceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ColorCode",
                table: "ServiceTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IconClass",
                table: "ServiceTypes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ColorCode",
                table: "ServiceTypes");

            migrationBuilder.DropColumn(
                name: "IconClass",
                table: "ServiceTypes");
        }
    }
}
