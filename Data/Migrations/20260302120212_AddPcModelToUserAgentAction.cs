using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPcModelToUserAgentAction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PcId",
                table: "UserAgentActions",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "UserAgentActions",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_UserAgentActions_PcId",
                table: "UserAgentActions",
                column: "PcId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserAgentActions_Pcs_PcId",
                table: "UserAgentActions",
                column: "PcId",
                principalTable: "Pcs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserAgentActions_Pcs_PcId",
                table: "UserAgentActions");

            migrationBuilder.DropIndex(
                name: "IX_UserAgentActions_PcId",
                table: "UserAgentActions");

            migrationBuilder.DropColumn(
                name: "PcId",
                table: "UserAgentActions");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "UserAgentActions");
        }
    }
}
