using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AnydeskTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class MetricaCollectionSettingsRevenueType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RevenueThreshold",
                table: "MetrikaCollectionSettings",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "REAL");

            migrationBuilder.UpdateData(
                table: "MetrikaCollectionSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "RevenueThreshold",
                value: 100m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "RevenueThreshold",
                table: "MetrikaCollectionSettings",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.UpdateData(
                table: "MetrikaCollectionSettings",
                keyColumn: "Id",
                keyValue: 1,
                column: "RevenueThreshold",
                value: 100.0);
        }
    }
}
