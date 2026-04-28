using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddDepotToBonLivraison : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepotId",
                table: "BON_LIVRAISON",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_BON_LIVRAISON_DepotId",
                table: "BON_LIVRAISON",
                column: "DepotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BON_LIVRAISON_Depots_DepotId",
                table: "BON_LIVRAISON",
                column: "DepotId",
                principalTable: "Depots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BON_LIVRAISON_Depots_DepotId",
                table: "BON_LIVRAISON");

            migrationBuilder.DropIndex(
                name: "IX_BON_LIVRAISON_DepotId",
                table: "BON_LIVRAISON");

            migrationBuilder.DropColumn(
                name: "DepotId",
                table: "BON_LIVRAISON");
        }
    }
}
