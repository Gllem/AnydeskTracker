using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class PcAhkError : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AhkError",
                table: "Pcs",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AhkError",
                table: "Pcs");
        }
    }
}
