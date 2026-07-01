using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase1_CreateCoreStockTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Depots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresse = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EstActif = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Depots", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Familles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Libelle = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Familles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AlertesStock",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    DepotId = table.Column<int>(type: "int", nullable: false),
                    NiveauAlerte = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    QuantiteAuDeclenchement = table.Column<int>(type: "int", nullable: false),
                    SeuilUtilise = table.Column<int>(type: "int", nullable: false),
                    EstResolue = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DateCreation = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    DateResolution = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlertesStock", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlertesStock_Depots_DepotId",
                        column: x => x.DepotId,
                        principalTable: "Depots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AlertesStock_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockQuantites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    DepotId = table.Column<int>(type: "int", nullable: false),
                    Quantite = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    SeuilMinimum = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastUpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockQuantites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockQuantites_Depots_DepotId",
                        column: x => x.DepotId,
                        principalTable: "Depots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockQuantites_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DepotDestinationId",
                table: "StockMovements",
                column: "DepotDestinationId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_DepotSourceId",
                table: "StockMovements",
                column: "DepotSourceId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_FamilleId",
                table: "Categories",
                column: "FamilleId");

            migrationBuilder.CreateIndex(
                name: "IX_AlertesStock_ArticleId_DepotId",
                table: "AlertesStock",
                columns: new[] { "ArticleId", "DepotId" });

            migrationBuilder.CreateIndex(
                name: "IX_AlertesStock_DepotId",
                table: "AlertesStock",
                column: "DepotId");

            migrationBuilder.CreateIndex(
                name: "IX_Depots_Nom",
                table: "Depots",
                column: "Nom",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Familles_Code",
                table: "Familles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockQuantites_ArticleId_DepotId",
                table: "StockQuantites",
                columns: new[] { "ArticleId", "DepotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockQuantites_DepotId",
                table: "StockQuantites",
                column: "DepotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Familles_FamilleId",
                table: "Categories",
                column: "FamilleId",
                principalTable: "Familles",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Depots_DepotDestinationId",
                table: "StockMovements",
                column: "DepotDestinationId",
                principalTable: "Depots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Depots_DepotSourceId",
                table: "StockMovements",
                column: "DepotSourceId",
                principalTable: "Depots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // ── Seed: Default Famille & Depot ─────────────────────────────────
            // MLD §2.1: Every system needs at least one family/depot to start.
            // Categories without a FamilleId will be manually linked to GENERAL later (Step 1C).
            migrationBuilder.Sql(
                "INSERT INTO `Familles` (`Code`, `Libelle`, `Description`, `CreatedAt`) " +
                "VALUES ('GENERAL', 'Équipements Généraux', 'Famille par défaut pour les articles non classifiés.', NOW());");

            migrationBuilder.Sql(
                "INSERT INTO `Depots` (`Nom`, `Adresse`, `EstActif`, `CreatedAt`) " +
                "VALUES ('Dépôt Principal', 'Site principal', TRUE, NOW());");
            // ─────────────────────────────────────────────────────────────────
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Familles_FamilleId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Depots_DepotDestinationId",
                table: "StockMovements");

            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Depots_DepotSourceId",
                table: "StockMovements");

            migrationBuilder.DropTable(
                name: "AlertesStock");

            migrationBuilder.DropTable(
                name: "Familles");

            migrationBuilder.DropTable(
                name: "StockQuantites");

            migrationBuilder.DropTable(
                name: "Depots");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_DepotDestinationId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_StockMovements_DepotSourceId",
                table: "StockMovements");

            migrationBuilder.DropIndex(
                name: "IX_Categories_FamilleId",
                table: "Categories");
        }
    }
}
