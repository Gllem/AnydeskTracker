using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkSessionModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkSessionModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkSessionModels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PcUsages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PcId = table.Column<int>(type: "INTEGER", nullable: false),
                    WorkSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcUsages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PcUsages_Pcs_PcId",
                        column: x => x.PcId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PcUsages_WorkSessionModels_WorkSessionId",
                        column: x => x.WorkSessionId,
                        principalTable: "WorkSessionModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PcUsages_PcId",
                table: "PcUsages",
                column: "PcId");

            migrationBuilder.CreateIndex(
                name: "IX_PcUsages_WorkSessionId",
                table: "PcUsages",
                column: "WorkSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSessionModels_UserId",
                table: "WorkSessionModels",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PcUsages");

            migrationBuilder.DropTable(
                name: "WorkSessionModels");
        }
    }
}
