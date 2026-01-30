using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class FkFixBotGameOrdersOverride : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotGameOrdersOverride_Pcs_PcModelId",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropIndex(
                name: "IX_BotGameOrdersOverride_PcModelId",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropColumn(
                name: "PcModelId",
                table: "BotGameOrdersOverride");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PcModelId",
                table: "BotGameOrdersOverride",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BotGameOrdersOverride_PcModelId",
                table: "BotGameOrdersOverride",
                column: "PcModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameOrdersOverride_Pcs_PcModelId",
                table: "BotGameOrdersOverride",
                column: "PcModelId",
                principalTable: "Pcs",
                principalColumn: "Id");
        }
    }
}
