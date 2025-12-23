using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class PcModelOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Pcs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
            
            
            migrationBuilder.Sql(@"
                WITH Ordered AS (
                    SELECT
                        Id,
                        ROW_NUMBER() OVER (ORDER BY Id) - 1 AS rn
                    FROM Pcs
                )
                UPDATE Pcs
                SET SortOrder = (
                    SELECT rn
                    FROM Ordered
                    WHERE Ordered.Id = Pcs.Id
                );
                ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Pcs");
        }
    }
}
