using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase5_CreateEavTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CHAMP_SPECIFIQUE",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CategorieId = table.Column<int>(type: "int", nullable: false),
                    NomChamp = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TypeDonnee = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "TEXTE")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Obligatoire = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Ordre = table.Column<int>(type: "int", nullable: false, defaultValue: 0)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CHAMP_SPECIFIQUE", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CHAMP_SPECIFIQUE_Categories_CategorieId",
                        column: x => x.CategorieId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ARTICLE_CHAMP_VALEUR",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    ChampSpecifiqueId = table.Column<int>(type: "int", nullable: false),
                    Valeur = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ARTICLE_CHAMP_VALEUR", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ARTICLE_CHAMP_VALEUR_CHAMP_SPECIFIQUE_ChampSpecifiqueId",
                        column: x => x.ChampSpecifiqueId,
                        principalTable: "CHAMP_SPECIFIQUE",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ARTICLE_CHAMP_VALEUR_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_CHAMP_VALEUR_ArticleId_ChampSpecifiqueId",
                table: "ARTICLE_CHAMP_VALEUR",
                columns: new[] { "ArticleId", "ChampSpecifiqueId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ARTICLE_CHAMP_VALEUR_ChampSpecifiqueId",
                table: "ARTICLE_CHAMP_VALEUR",
                column: "ChampSpecifiqueId");

            migrationBuilder.CreateIndex(
                name: "IX_CHAMP_SPECIFIQUE_CategorieId",
                table: "CHAMP_SPECIFIQUE",
                column: "CategorieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ARTICLE_CHAMP_VALEUR");

            migrationBuilder.DropTable(
                name: "CHAMP_SPECIFIQUE");
        }
    }
}
