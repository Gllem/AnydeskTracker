using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddNewGameModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GameCatalog",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameCatalog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BotGameOrderGlobal",
                columns: table => new
                {
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotGameOrderGlobal", x => x.GameId);
                    table.ForeignKey(
                        name: "FK_BotGameOrderGlobal_GameCatalog_GameId",
                        column: x => x.GameId,
                        principalTable: "GameCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BotGameOrderOverride",
                columns: table => new
                {
                    PcId = table.Column<int>(type: "INTEGER", nullable: false),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false),
                    PcModelId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotGameOrderOverride", x => new { x.PcId, x.GameId });
                    table.ForeignKey(
                        name: "FK_BotGameOrderOverride_GameCatalog_GameId",
                        column: x => x.GameId,
                        principalTable: "GameCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotGameOrderOverride_Pcs_PcId",
                        column: x => x.PcId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotGameOrderOverride_Pcs_PcModelId",
                        column: x => x.PcModelId,
                        principalTable: "Pcs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameSchedule",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<int>(type: "INTEGER", nullable: false),
                    DayOfWeek = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSchedule_GameCatalog_GameId",
                        column: x => x.GameId,
                        principalTable: "GameCatalog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameUserScheduleToUser",
                columns: table => new
                {
                    GameUserScheduleId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    AppUserId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameUserScheduleToUser", x => new { x.GameUserScheduleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GameUserScheduleToUser_AspNetUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameUserScheduleToUser_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameUserScheduleToUser_GameSchedule_GameUserScheduleId",
                        column: x => x.GameUserScheduleId,
                        principalTable: "GameSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotGameOrderOverride_GameId",
                table: "BotGameOrderOverride",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BotGameOrderOverride_PcModelId",
                table: "BotGameOrderOverride",
                column: "PcModelId");

            migrationBuilder.CreateIndex(
                name: "IX_GameCatalog_Url",
                table: "GameCatalog",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSchedule_GameId",
                table: "GameSchedule",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameUserScheduleToUser_AppUserId",
                table: "GameUserScheduleToUser",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameUserScheduleToUser_UserId",
                table: "GameUserScheduleToUser",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotGameOrderGlobal");

            migrationBuilder.DropTable(
                name: "BotGameOrderOverride");

            migrationBuilder.DropTable(
                name: "GameUserScheduleToUser");

            migrationBuilder.DropTable(
                name: "GameSchedule");

            migrationBuilder.DropTable(
                name: "GameCatalog");
        }
    }
}
