using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase4_CreateOrderAndDeliveryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BON_COMMANDE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumeroBC = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    DateCommande = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    Statut = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false, defaultValue: "EN_ATTENTE")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Commentaire = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BON_COMMANDE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BON_COMMANDE_CLIENT_ClientId",
                        column: x => x.ClientId,
                        principalTable: "CLIENT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BON_LIVRAISON",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    NumeroBL = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BonCommandeId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    DateLivraison = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    Statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "BROUILLON")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdresseLivraison = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Transporteur = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumeroSuivi = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BON_LIVRAISON", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BON_LIVRAISON_BON_COMMANDE_BonCommandeId",
                        column: x => x.BonCommandeId,
                        principalTable: "BON_COMMANDE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BON_LIVRAISON_CLIENT_ClientId",
                        column: x => x.ClientId,
                        principalTable: "CLIENT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LIGNE_BC",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BonCommandeId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    QuantiteCommandee = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LIGNE_BC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LIGNE_BC_BON_COMMANDE_BonCommandeId",
                        column: x => x.BonCommandeId,
                        principalTable: "BON_COMMANDE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LIGNE_BC_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LIGNE_BL",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BonLivraisonId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    QuantiteLivree = table.Column<int>(type: "int", nullable: false),
                    LotId = table.Column<int>(type: "int", nullable: true),
                    NumeroSerie = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LIGNE_BL", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LIGNE_BL_BON_LIVRAISON_BonLivraisonId",
                        column: x => x.BonLivraisonId,
                        principalTable: "BON_LIVRAISON",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LIGNE_BL_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_BON_COMMANDE_ClientId",
                table: "BON_COMMANDE",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BON_COMMANDE_NumeroBC",
                table: "BON_COMMANDE",
                column: "NumeroBC",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BON_LIVRAISON_BonCommandeId",
                table: "BON_LIVRAISON",
                column: "BonCommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_BON_LIVRAISON_ClientId",
                table: "BON_LIVRAISON",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_BON_LIVRAISON_NumeroBL",
                table: "BON_LIVRAISON",
                column: "NumeroBL",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LIGNE_BC_ArticleId",
                table: "LIGNE_BC",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_LIGNE_BC_BonCommandeId",
                table: "LIGNE_BC",
                column: "BonCommandeId");

            migrationBuilder.CreateIndex(
                name: "IX_LIGNE_BL_ArticleId",
                table: "LIGNE_BL",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_LIGNE_BL_BonLivraisonId",
                table: "LIGNE_BL",
                column: "BonLivraisonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LIGNE_BC");

            migrationBuilder.DropTable(
                name: "LIGNE_BL");

            migrationBuilder.DropTable(
                name: "BON_LIVRAISON");

            migrationBuilder.DropTable(
                name: "BON_COMMANDE");
        }
    }
}
