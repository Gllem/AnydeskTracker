using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class BlockedAgentCredsRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedAgentCredentialsModel");

            migrationBuilder.CreateTable(
                name: "BlockedAgentEmails",
                columns: table => new
                {
                    Email = table.Column<string>(type: "TEXT", maxLength: 320, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedAgentEmails", x => x.Email);
                });

            migrationBuilder.CreateTable(
                name: "BlockedAgentPhones",
                columns: table => new
                {
                    Phone = table.Column<string>(type: "TEXT", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedAgentPhones", x => x.Phone);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockedAgentEmails");

            migrationBuilder.DropTable(
                name: "BlockedAgentPhones");

            migrationBuilder.CreateTable(
                name: "BlockedAgentCredentialsModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Credentials = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockedAgentCredentialsModel", x => x.Id);
                });
        }
    }
}
