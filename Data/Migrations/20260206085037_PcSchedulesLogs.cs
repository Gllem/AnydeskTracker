using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class PcSchedulesLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "LastActiveSchedule_Enabled",
                table: "Pcs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LastActiveSchedule_EndTod",
                table: "Pcs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastActiveSchedule_IntervalMinutes",
                table: "Pcs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "LastActiveSchedule_StartTod",
                table: "Pcs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastLaunchTime",
                table: "Pcs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastStatus",
                table: "Pcs",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "NextLaunchTime",
                table: "Pcs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveSchedule_Enabled",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "LastActiveSchedule_EndTod",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "LastActiveSchedule_IntervalMinutes",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "LastActiveSchedule_StartTod",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "LastLaunchTime",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "LastStatus",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "NextLaunchTime",
                table: "Pcs");
        }
    }
}
