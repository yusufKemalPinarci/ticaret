using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotebookTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TaxRatesAndOrderTaxFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceUrl",
                table: "Orders",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxRegion",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "OrderItems",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TaxRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RatePercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxRates", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaxRates_Region",
                table: "TaxRates",
                column: "Region",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaxRates");

            migrationBuilder.DropColumn(
                name: "InvoiceUrl",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxRegion",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "OrderItems");
        }
    }
}
