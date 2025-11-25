using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class PausableModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "WorkSessionModels",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PauseStartTime",
                table: "WorkSessionModels",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalPauseTime",
                table: "WorkSessionModels",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<bool>(
                name: "IsPaused",
                table: "PcUsages",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "PauseStartTime",
                table: "PcUsages",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TotalPauseTime",
                table: "PcUsages",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "WorkSessionModels");

            migrationBuilder.DropColumn(
                name: "PauseStartTime",
                table: "WorkSessionModels");

            migrationBuilder.DropColumn(
                name: "TotalPauseTime",
                table: "WorkSessionModels");

            migrationBuilder.DropColumn(
                name: "IsPaused",
                table: "PcUsages");

            migrationBuilder.DropColumn(
                name: "PauseStartTime",
                table: "PcUsages");

            migrationBuilder.DropColumn(
                name: "TotalPauseTime",
                table: "PcUsages");
        }
    }
}
