using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class PcSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Pcs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<bool>(
                name: "PcBotSchedule_Enabled",
                table: "Pcs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PcBotSchedule_EndTod",
                table: "Pcs",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PcBotSchedule_IntervalMinutes",
                table: "Pcs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "PcBotSchedule_StartTod",
                table: "Pcs",
                type: "TEXT",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PcBotSchedule_Enabled",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "PcBotSchedule_EndTod",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "PcBotSchedule_IntervalMinutes",
                table: "Pcs");

            migrationBuilder.DropColumn(
                name: "PcBotSchedule_StartTod",
                table: "Pcs");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Pcs",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);
        }
    }
}
