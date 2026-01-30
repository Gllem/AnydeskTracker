using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemovedLegacyModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrderGlobal_GameCatalog_GameId",
                table: "BotGameOrderGlobal");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrderOverride_GameCatalog_GameId",
                table: "BotGameOrderOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrderOverride_Pcs_PcId",
                table: "BotGameOrderOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrderOverride_Pcs_PcModelId",
                table: "BotGameOrderOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSchedule_GameCatalog_GameId",
                table: "GameSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_GameUserScheduleToUser_GameSchedule_GameUserScheduleId",
                table: "GameUserScheduleToUser");

            migrationBuilder.DropTable(
                name: "GameUserSchedulesUsers");

            migrationBuilder.DropTable(
                name: "PcModelToBotGames");

            migrationBuilder.DropTable(
                name: "GameUserSchedule");

            migrationBuilder.DropTable(
                name: "BotGames");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameSchedule",
                table: "GameSchedule");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameOrderOverride",
                table: "BotGameOrderOverride");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameOrderGlobal",
                table: "BotGameOrderGlobal");

            migrationBuilder.RenameTable(
                name: "GameSchedule",
                newName: "GameSchedules");

            migrationBuilder.RenameTable(
                name: "BotGameOrderOverride",
                newName: "BotGameOrdersOverride");

            migrationBuilder.RenameTable(
                name: "BotGameOrderGlobal",
                newName: "BotGameOrdersGlobal");

            migrationBuilder.RenameIndex(
                name: "IX_GameSchedule_GameId",
                table: "GameSchedules",
                newName: "IX_GameSchedules_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_BotGameOrderOverride_PcModelId",
                table: "BotGameOrdersOverride",
                newName: "IX_BotGameOrdersOverride_PcModelId");

            migrationBuilder.RenameIndex(
                name: "IX_BotGameOrderOverride_GameId",
                table: "BotGameOrdersOverride",
                newName: "IX_BotGameOrdersOverride_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameSchedules",
                table: "GameSchedules",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotGameOrdersOverride",
                table: "BotGameOrdersOverride",
                columns: new[] { "PcId", "GameId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotGameOrdersGlobal",
                table: "BotGameOrdersGlobal",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrdersGlobal_GameCatalog_GameId",
                table: "BotGameOrdersGlobal",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrdersOverride_GameCatalog_GameId",
                table: "BotGameOrdersOverride",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrdersOverride_Pcs_PcId",
                table: "BotGameOrdersOverride",
                column: "PcId",
                principalTable: "Pcs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrdersOverride_Pcs_PcModelId",
                table: "BotGameOrdersOverride",
                column: "PcModelId",
                principalTable: "Pcs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSchedules_GameCatalog_GameId",
                table: "GameSchedules",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameUserScheduleToUser_GameSchedules_GameUserScheduleId",
                table: "GameUserScheduleToUser",
                column: "GameUserScheduleId",
                principalTable: "GameSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrdersGlobal_GameCatalog_GameId",
                table: "BotGameOrdersGlobal");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrdersOverride_GameCatalog_GameId",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrdersOverride_Pcs_PcId",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrdersOverride_Pcs_PcModelId",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSchedules_GameCatalog_GameId",
                table: "GameSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_GameUserScheduleToUser_GameSchedules_GameUserScheduleId",
                table: "GameUserScheduleToUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameSchedules",
                table: "GameSchedules");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameOrdersOverride",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameOrdersGlobal",
                table: "BotGameOrdersGlobal");

            migrationBuilder.RenameTable(
                name: "GameSchedules",
                newName: "GameSchedule");

            migrationBuilder.RenameTable(
                name: "BotGameOrdersOverride",
                newName: "BotGameOrderOverride");

            migrationBuilder.RenameTable(
                name: "BotGameOrdersGlobal",
                newName: "BotGameOrderGlobal");

            migrationBuilder.RenameIndex(
                name: "IX_GameSchedules_GameId",
                table: "GameSchedule",
                newName: "IX_GameSchedule_GameId");

            migrationBuilder.RenameIndex(
                name: "IX_BotGameOrdersOverride_PcModelId",
                table: "BotGameOrderOverride",
                newName: "IX_BotGameOrderOverride_PcModelId");

            migrationBuilder.RenameIndex(
                name: "IX_BotGameOrdersOverride_GameId",
                table: "BotGameOrderOverride",
                newName: "IX_BotGameOrderOverride_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameSchedule",
                table: "GameSchedule",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotGameOrderOverride",
                table: "BotGameOrderOverride",
                columns: new[] { "PcId", "GameId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotGameOrderGlobal",
                table: "BotGameOrderGlobal",
                column: "GameId");

            migrationBuilder.CreateTable(
                name: "BotGames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameUrl = table.Column<string>(type: "TEXT", nullable: false),
                    GlobalOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsGlobal = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameName = table.Column<string>(type: "TEXT", nullable: false),
                    GameUrl = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PcModelToBotGames",
                columns: table => new
                {
                    PcModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    BotGameId = table.Column<int>(type: "INTEGER", nullable: false),
                    Order = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PcModelToBotGames", x => new { x.PcModelId, x.BotGameId });
                    table.ForeignKey(
                        name: "FK_PcModelToBotGames_BotGames_BotGameId",
                        column: x => x.BotGameId,
                        principalTable: "BotGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PcModelToBotGames_Pcs_PcModelId",
                        column: x => x.PcModelId,
                        principalTable: "Pcs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_PcModelToBotGames_BotGameId",
                table: "PcModelToBotGames",
                column: "BotGameId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrderGlobal_GameCatalog_GameId",
                table: "BotGameOrderGlobal",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrderOverride_GameCatalog_GameId",
                table: "BotGameOrderOverride",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrderOverride_Pcs_PcId",
                table: "BotGameOrderOverride",
                column: "PcId",
                principalTable: "Pcs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrderOverride_Pcs_PcModelId",
                table: "BotGameOrderOverride",
                column: "PcModelId",
                principalTable: "Pcs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSchedule_GameCatalog_GameId",
                table: "GameSchedule",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GameUserScheduleToUser_GameSchedule_GameUserScheduleId",
                table: "GameUserScheduleToUser",
                column: "GameUserScheduleId",
                principalTable: "GameSchedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
