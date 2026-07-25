using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class fixCostomerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CusromerId",
                table: "SalesInvoices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CusromerId",
                table: "SalesInvoices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
