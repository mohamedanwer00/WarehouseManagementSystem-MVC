using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCustomerTransactionType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BalanceType",
                table: "SupplierTransactions",
                newName: "SupplierTransactionType");

            migrationBuilder.RenameColumn(
                name: "BalanceType",
                table: "CustomerTransactions",
                newName: "CustomerTransactionType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupplierTransactionType",
                table: "SupplierTransactions",
                newName: "BalanceType");

            migrationBuilder.RenameColumn(
                name: "CustomerTransactionType",
                table: "CustomerTransactions",
                newName: "BalanceType");
        }
    }
}
