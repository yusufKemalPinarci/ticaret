using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotebookTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ShippingRatesAndTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingRegion",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingWeight",
                table: "Orders",
                type: "decimal(18,3)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TrackingNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ShippingRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    WeightFrom = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    WeightTo = table.Column<decimal>(type: "decimal(18,3)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingRates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShippingRates_Region_WeightFrom_WeightTo",
                table: "ShippingRates",
                columns: new[] { "Region", "WeightFrom", "WeightTo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShippingRates");

            migrationBuilder.DropColumn(
                name: "ShippingRegion",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingWeight",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TrackingNumber",
                table: "Orders");
        }
    }
}
