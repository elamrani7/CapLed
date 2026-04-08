using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase2_AddLotAndSerialFkToMovements : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LotId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumeroSerieId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_LotId",
                table: "StockMovements",
                column: "LotId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_NumeroSerieId",
                table: "StockMovements",
                column: "NumeroSerieId");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_LOT_LotId",
                table: "StockMovements",
                column: "LotId",
                principalTable: "LOT",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_NUMERO_SERIE_NumeroSerieId",
                table: "StockMovements",
                column: "NumeroSerieId",
                principalTable: "NUMERO_SERIE",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_LOT_LotId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_NUMERO_SERIE_NumeroSerieId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_LotId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_NumeroSerieId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "LotId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "NumeroSerieId",
                table: "StockMovements");
        }
    }
}
