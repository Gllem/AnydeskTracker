using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class GameDataCache : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GameName",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GamePictureUrl",
                table: "Games",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameName",
                table: "Games");

            migrationBuilder.DropColumn(
                name: "GamePictureUrl",
                table: "Games");
        }
    }
}
