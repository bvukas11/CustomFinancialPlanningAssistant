using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CustomFinancialPlanningAssistant.Migrations
{
    /// <inheritdoc />
    public partial class AddIndustryBenchmarking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IndustryBenchmarks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Industry = table.Column<int>(type: "int", nullable: false),
                    MetricName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AverageValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    MedianValue = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Percentile25 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    Percentile75 = table.Column<decimal>(type: "decimal(18,4)", precision: 18, scale: 4, nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    DataSource = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, defaultValue: "Industry Standard"),
                    SampleSize = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryBenchmarks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_IndustryBenchmarks_Industry",
                table: "IndustryBenchmarks",
                column: "Industry");

            migrationBuilder.CreateIndex(
                name: "IX_IndustryBenchmarks_Industry_MetricName",
                table: "IndustryBenchmarks",
                columns: new[] { "Industry", "MetricName" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryBenchmarks_LastUpdated",
                table: "IndustryBenchmarks",
                column: "LastUpdated",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryBenchmarks_MetricName",
                table: "IndustryBenchmarks",
                column: "MetricName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IndustryBenchmarks");
        }
    }
}
