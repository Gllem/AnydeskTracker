using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedBrowserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BrowserRevenues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Browser = table.Column<string>(type: "TEXT", nullable: false),
                    LastRevenue = table.Column<decimal>(type: "TEXT", nullable: false),
                    DeltaRevenue = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BrowserRevenues", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "BrowserRevenues",
                columns: new[] { "Id", "Browser", "DeltaRevenue", "LastRevenue" },
                values: new object[,]
                {
                    { 1, "Chrome", 0m, 0m },
                    { 2, "Edge", 0m, 0m },
                    { 3, "Firefox", 0m, 0m },
                    { 4, "Opera", 0m, 0m },
                    { 6, "YandexBrowser", 0m, 0m },
                    { 7, "DuckDuckGo", 0m, 0m },
                    { 8, "Dolphin", 0m, 0m },
                    { 9, "Octo", 0m, 0m },
                    { 10, "Brave", 0m, 0m },
                    { 11, "Atom", 0m, 0m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BrowserRevenues");
        }
    }
}
