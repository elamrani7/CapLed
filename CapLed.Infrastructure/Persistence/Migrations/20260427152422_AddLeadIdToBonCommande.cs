using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLeadIdToBonCommande : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "BON_COMMANDE",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BON_COMMANDE_LeadId",
                table: "BON_COMMANDE",
                column: "LeadId");

            migrationBuilder.AddForeignKey(
                name: "FK_BON_COMMANDE_LEAD_LeadId",
                table: "BON_COMMANDE",
                column: "LeadId",
                principalTable: "LEAD",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BON_COMMANDE_LEAD_LeadId",
                table: "BON_COMMANDE");

            migrationBuilder.DropIndex(
                name: "IX_BON_COMMANDE_LeadId",
                table: "BON_COMMANDE");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "BON_COMMANDE");
        }
    }
}
