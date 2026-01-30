using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class FkFixGameUserScheduleToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameUserScheduleToUser_AspNetUsers_AppUserId",
                table: "GameUserScheduleToUser");

            migrationBuilder.DropIndex(
                name: "IX_GameUserScheduleToUser_AppUserId",
                table: "GameUserScheduleToUser");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "GameUserScheduleToUser");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AppUserId",
                table: "GameUserScheduleToUser",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameUserScheduleToUser_AppUserId",
                table: "GameUserScheduleToUser",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameUserScheduleToUser_AspNetUsers_AppUserId",
                table: "GameUserScheduleToUser",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
