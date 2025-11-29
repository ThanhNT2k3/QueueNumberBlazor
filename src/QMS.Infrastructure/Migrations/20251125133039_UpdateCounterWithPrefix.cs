using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QMS.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCounterWithPrefix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DailySequence",
                table: "Counters",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastResetDate",
                table: "Counters",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "Counters",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DailySequence",
                table: "Counters");

            migrationBuilder.DropColumn(
                name: "LastResetDate",
                table: "Counters");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "Counters");
        }
    }
}
