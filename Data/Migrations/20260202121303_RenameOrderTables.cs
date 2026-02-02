using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameOrderTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameOrdersOverride",
                table: "BotGameOrdersOverride");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameOrdersGlobal",
                table: "BotGameOrdersGlobal");

            migrationBuilder.RenameTable(
                name: "BotGameOrdersOverride",
                newName: "BotGameAssignmentsOverride");

            migrationBuilder.RenameTable(
                name: "BotGameOrdersGlobal",
                newName: "BotGameAssignmentsGlobal");

            migrationBuilder.RenameIndex(
                name: "IX_BotGameOrdersOverride_GameId",
                table: "BotGameAssignmentsOverride",
                newName: "IX_BotGameAssignmentsOverride_GameId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotGameAssignmentsOverride",
                table: "BotGameAssignmentsOverride",
                columns: new[] { "PcId", "GameId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_BotGameAssignmentsGlobal",
                table: "BotGameAssignmentsGlobal",
                column: "GameId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameAssignmentsGlobal_GameCatalog_GameId",
                table: "BotGameAssignmentsGlobal",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameAssignmentsOverride_GameCatalog_GameId",
                table: "BotGameAssignmentsOverride",
                column: "GameId",
                principalTable: "GameCatalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BotGameAssignmentsOverride_Pcs_PcId",
                table: "BotGameAssignmentsOverride",
                column: "PcId",
                principalTable: "Pcs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotGameAssignmentsGlobal_GameCatalog_GameId",
                table: "BotGameAssignmentsGlobal");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameAssignmentsOverride_GameCatalog_GameId",
                table: "BotGameAssignmentsOverride");

            migrationBuilder.DropForeignKey(
                name: "FK_BotGameAssignmentsOverride_Pcs_PcId",
                table: "BotGameAssignmentsOverride");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameAssignmentsOverride",
                table: "BotGameAssignmentsOverride");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BotGameAssignmentsGlobal",
                table: "BotGameAssignmentsGlobal");

            migrationBuilder.RenameTable(
                name: "BotGameAssignmentsOverride",
                newName: "BotGameOrdersOverride");

            migrationBuilder.RenameTable(
                name: "BotGameAssignmentsGlobal",
                newName: "BotGameOrdersGlobal");

            migrationBuilder.RenameIndex(
                name: "IX_BotGameAssignmentsOverride_GameId",
                table: "BotGameOrdersOverride",
                newName: "IX_BotGameOrdersOverride_GameId");

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
        }
    }
}
