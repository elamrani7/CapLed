using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase2_CreateLotAndNumeroSerieTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LOT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    DepotId = table.Column<int>(type: "int", nullable: false),
                    NumeroLot = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantite = table.Column<int>(type: "int", nullable: false),
                    Fournisseur = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateEntree = table.Column<DateTime>(type: "DATE", nullable: false),
                    Garantie = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Certificat = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LOT", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LOT_Depots_DepotId",
                        column: x => x.DepotId,
                        principalTable: "Depots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LOT_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NUMERO_SERIE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    DepotId = table.Column<int>(type: "int", nullable: false),
                    NumeroSerieLabel = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateEntree = table.Column<DateTime>(type: "DATE", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NUMERO_SERIE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NUMERO_SERIE_Depots_DepotId",
                        column: x => x.DepotId,
                        principalTable: "Depots",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NUMERO_SERIE_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LOT_ArticleId",
                table: "LOT",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_LOT_DepotId",
                table: "LOT",
                column: "DepotId");

            migrationBuilder.CreateIndex(
                name: "IX_LOT_NumeroLot_ArticleId_DepotId",
                table: "LOT",
                columns: new[] { "NumeroLot", "ArticleId", "DepotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NUMERO_SERIE_ArticleId",
                table: "NUMERO_SERIE",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_NUMERO_SERIE_DepotId",
                table: "NUMERO_SERIE",
                column: "DepotId");

            migrationBuilder.CreateIndex(
                name: "IX_NUMERO_SERIE_NumeroSerieLabel",
                table: "NUMERO_SERIE",
                column: "NumeroSerieLabel",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LOT");

            migrationBuilder.DropTable(
                name: "NUMERO_SERIE");
        }
    }
}
