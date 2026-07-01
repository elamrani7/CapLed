using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase1_AddColumnsToExisting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DepotDestinationId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DepotSourceId",
                table: "StockMovements",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeMouvement",
                table: "StockMovements",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "ENTREE")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ArticleSimilaireIds",
                table: "Equipments",
                type: "TEXT",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DatasheetUrl",
                table: "Equipments",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "DisponibiliteSite",
                table: "Equipments",
                type: "varchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "EN_STOCK")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "PrixAchat",
                table: "Equipments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PrixVente",
                table: "Equipments",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "VisibleSite",
                table: "Equipments",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FamilleId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TypeGestionStock",
                table: "Categories",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "QUANTITE")
                .Annotation("MySql:CharSet", "utf8mb4");

            // ── Data migration: normalize legacy role value ────────────────────
            // MLD §2.3: new RBAC model has ADMIN, COMMERCIAL, TECHNICIEN.
            // STOCK_MANAGER is kept as [Obsolete] in the C# enum for compat.
            migrationBuilder.Sql(
                "UPDATE `Users` SET `Role` = 'TECHNICIEN' WHERE `Role` = 'STOCK_MANAGER';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revert data migration (best-effort)
            migrationBuilder.Sql(
                "UPDATE `Users` SET `Role` = 'STOCK_MANAGER' WHERE `Role` = 'TECHNICIEN';");

            migrationBuilder.DropColumn(
                name: "DepotDestinationId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "DepotSourceId",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "TypeMouvement",
                table: "StockMovements");

            migrationBuilder.DropColumn(
                name: "ArticleSimilaireIds",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DatasheetUrl",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "DisponibiliteSite",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "PrixAchat",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "PrixVente",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "VisibleSite",
                table: "Equipments");

            migrationBuilder.DropColumn(
                name: "FamilleId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TypeGestionStock",
                table: "Categories");
        }
    }
}
