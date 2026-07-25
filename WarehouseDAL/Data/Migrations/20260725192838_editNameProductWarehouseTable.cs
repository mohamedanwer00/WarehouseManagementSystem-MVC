using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WarehouseDAL.Data.Migrations
{
    /// <inheritdoc />
    public partial class editNameProductWarehouseTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductWarehouse_Products_ProductId",
                table: "ProductWarehouse");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWarehouse_Warehouses_WarehouseId",
                table: "ProductWarehouse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductWarehouse",
                table: "ProductWarehouse");

            migrationBuilder.RenameTable(
                name: "ProductWarehouse",
                newName: "ProductWarehouses");

            migrationBuilder.RenameIndex(
                name: "IX_ProductWarehouse_WarehouseId",
                table: "ProductWarehouses",
                newName: "IX_ProductWarehouses_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductWarehouse_ProductId_WarehouseId",
                table: "ProductWarehouses",
                newName: "IX_ProductWarehouses_ProductId_WarehouseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductWarehouses",
                table: "ProductWarehouses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWarehouses_Products_ProductId",
                table: "ProductWarehouses",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWarehouses_Warehouses_WarehouseId",
                table: "ProductWarehouses",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductWarehouses_Products_ProductId",
                table: "ProductWarehouses");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductWarehouses_Warehouses_WarehouseId",
                table: "ProductWarehouses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductWarehouses",
                table: "ProductWarehouses");

            migrationBuilder.RenameTable(
                name: "ProductWarehouses",
                newName: "ProductWarehouse");

            migrationBuilder.RenameIndex(
                name: "IX_ProductWarehouses_WarehouseId",
                table: "ProductWarehouse",
                newName: "IX_ProductWarehouse_WarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductWarehouses_ProductId_WarehouseId",
                table: "ProductWarehouse",
                newName: "IX_ProductWarehouse_ProductId_WarehouseId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductWarehouse",
                table: "ProductWarehouse",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWarehouse_Products_ProductId",
                table: "ProductWarehouse",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductWarehouse_Warehouses_WarehouseId",
                table: "ProductWarehouse",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
