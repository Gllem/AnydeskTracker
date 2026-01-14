using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPcModelBotGamesRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsGlobal",
                table: "BotGames",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PcModelToBotGames",
                columns: table => new
                {
                    PcModelId = table.Column<int>(type: "INTEGER", nullable: false),
                    BotGameId = table.Column<int>(type: "INTEGER", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_PcModelToBotGames_BotGameId",
                table: "PcModelToBotGames",
                column: "BotGameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PcModelToBotGames");

            migrationBuilder.DropColumn(
                name: "IsGlobal",
                table: "BotGames");
        }
    }
}
