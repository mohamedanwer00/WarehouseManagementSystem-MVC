using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSellingPriceToSalesInvoiceItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SellingPrice",
                table: "SalesInvoiceItems",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SellingPrice",
                table: "SalesInvoiceItems");
        }
    }
}
