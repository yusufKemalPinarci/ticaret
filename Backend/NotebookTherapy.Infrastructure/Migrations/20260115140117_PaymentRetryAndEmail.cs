using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotebookTherapy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PaymentRetryAndEmail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PaymentRetryCount",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentRetryCount",
                table: "Orders");
        }
    }
}
