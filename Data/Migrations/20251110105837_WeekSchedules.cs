using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class WeekSchedules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameUsers");

            migrationBuilder.CreateTable(
                name: "GameUserSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameUserSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameUserSchedule_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameUserSchedulesUsers",
                columns: table => new
                {
                    AssignedSchedulesId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameUserSchedulesUsers", x => new { x.AssignedSchedulesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GameUserSchedulesUsers_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameUserSchedulesUsers_GameUserSchedule_AssignedSchedulesId",
                        column: x => x.AssignedSchedulesId,
                        principalTable: "GameUserSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameUserSchedule_GameId",
                table: "GameUserSchedule",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameUserSchedulesUsers_UsersId",
                table: "GameUserSchedulesUsers",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameUserSchedulesUsers");

            migrationBuilder.DropTable(
                name: "GameUserSchedule");

            migrationBuilder.CreateTable(
                name: "GameUsers",
                columns: table => new
                {
                    AssignedGamesId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsersId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameUsers", x => new { x.AssignedGamesId, x.UsersId });
                    table.ForeignKey(
                        name: "FK_GameUsers_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameUsers_Games_AssignedGamesId",
                        column: x => x.AssignedGamesId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameUsers_UsersId",
                table: "GameUsers",
                column: "UsersId");
        }
    }
}
