using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserAgentActionModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserAgentActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    WorkSessionId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserLogType = table.Column<int>(type: "INTEGER", nullable: false),
                    AdditionalParams = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAgentActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAgentActions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAgentActions_WorkSessionModels_WorkSessionId",
                        column: x => x.WorkSessionId,
                        principalTable: "WorkSessionModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAgentActions_UserId",
                table: "UserAgentActions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAgentActions_WorkSessionId",
                table: "UserAgentActions",
                column: "WorkSessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserAgentActions");
        }
    }
}
