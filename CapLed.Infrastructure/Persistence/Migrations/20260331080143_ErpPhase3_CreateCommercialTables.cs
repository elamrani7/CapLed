using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase3_CreateCommercialTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CLIENT",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Prenom = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(150)", maxLength: 150, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Telephone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Societe = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Adresse = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CLIENT", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LEAD",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    CommercialId = table.Column<int>(type: "int", nullable: true),
                    NumeroDevis = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Statut = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "NOUVEAU")
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DateSoumission = table.Column<DateTime>(type: "datetime(6)", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP(6)"),
                    DateTraitement = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Commentaire = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SourceAcquisition = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false, defaultValue: "DIRECT")
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LEAD", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LEAD_CLIENT_ClientId",
                        column: x => x.ClientId,
                        principalTable: "CLIENT",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LEAD_Users_CommercialId",
                        column: x => x.CommercialId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LIGNE_LEAD",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LeadId = table.Column<int>(type: "int", nullable: false),
                    ArticleId = table.Column<int>(type: "int", nullable: false),
                    QuantiteDemandee = table.Column<int>(type: "int", nullable: false),
                    Commentaire = table.Column<string>(type: "TEXT", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LIGNE_LEAD", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LIGNE_LEAD_Equipments_ArticleId",
                        column: x => x.ArticleId,
                        principalTable: "Equipments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LIGNE_LEAD_LEAD_LeadId",
                        column: x => x.LeadId,
                        principalTable: "LEAD",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CLIENT_Email",
                table: "CLIENT",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LEAD_ClientId",
                table: "LEAD",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_LEAD_CommercialId",
                table: "LEAD",
                column: "CommercialId");

            migrationBuilder.CreateIndex(
                name: "IX_LEAD_NumeroDevis",
                table: "LEAD",
                column: "NumeroDevis",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LIGNE_LEAD_ArticleId",
                table: "LIGNE_LEAD",
                column: "ArticleId");

            migrationBuilder.CreateIndex(
                name: "IX_LIGNE_LEAD_LeadId",
                table: "LIGNE_LEAD",
                column: "LeadId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LIGNE_LEAD");

            migrationBuilder.DropTable(
                name: "LEAD");

            migrationBuilder.DropTable(
                name: "CLIENT");
        }
    }
}
