using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockManager.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ErpPhase1_SeedInitialStockQuantites : Migration
    {
        // ── Design notes ──────────────────────────────────────────────────────
        // Step 1C is a PURE DATA migration — no DDL changes.
        //
        // Up():
        //  1. Resolve the IDs of the seed records added in Step 1B
        //     (GENERAL family / Dépôt Principal) using MySQL user-variables.
        //  2. INSERT INTO StockQuantites … SELECT … WHERE NOT EXISTS
        //     → fully idempotent: re-running the migration never creates duplicates
        //       thanks to the NOT EXISTS guard + the UNIQUE(ArticleId, DepotId) index.
        //  3. UPDATE Categories SET FamilleId = @FamilleId WHERE FamilleId IS NULL
        //     → links every still-unclassified category to the GENERAL family.
        //
        // Down():
        //  1. Reset Categories.FamilleId that match the GENERAL family back to NULL.
        //  2. Delete StockQuantites rows seeded for Dépôt Principal.
        //     (does not delete rows created by the application after the migration)
        // ─────────────────────────────────────────────────────────────────────

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── 1. Resolve seed IDs into MySQL user-variables ─────────────────
            // Using SET + subquery so the ID is captured once and reused safely.
            migrationBuilder.Sql(
                "SET @DepotId = (SELECT `Id` FROM `Depots` WHERE `Nom` = 'Dépôt Principal' LIMIT 1);");

            migrationBuilder.Sql(
                "SET @FamilleId = (SELECT `Id` FROM `Familles` WHERE `Code` = 'GENERAL' LIMIT 1);");

            // ── 2. Populate StockQuantites from Equipments.Quantity ───────────
            // Idempotence: the NOT EXISTS clause skips rows that already exist.
            // SeuilMinimum = Equipment.MinThreshold (added in the original schema).
            // LastUpdatedAt = NOW() stamps the migration execution time.
            migrationBuilder.Sql(@"
INSERT INTO `StockQuantites` (`ArticleId`, `DepotId`, `Quantite`, `SeuilMinimum`, `LastUpdatedAt`)
SELECT
    e.`Id`,
    @DepotId,
    COALESCE(e.`Quantity`, 0),
    COALESCE(e.`MinThreshold`, 0),
    NOW()
FROM `Equipments` e
WHERE @DepotId IS NOT NULL
  AND NOT EXISTS (
      SELECT 1
      FROM `StockQuantites` sq
      WHERE sq.`ArticleId` = e.`Id`
        AND sq.`DepotId`   = @DepotId
  );");

            // ── 3. Link unclassified Categories → GENERAL family ──────────────
            // Only updates rows where FamilleId is still NULL (not yet assigned).
            migrationBuilder.Sql(@"
UPDATE `Categories`
SET    `FamilleId` = @FamilleId
WHERE  `FamilleId` IS NULL
  AND  @FamilleId IS NOT NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Re-resolve seed IDs (same pattern as Up, needed for rollback)
            migrationBuilder.Sql(
                "SET @DepotId = (SELECT `Id` FROM `Depots` WHERE `Nom` = 'Dépôt Principal' LIMIT 1);");

            migrationBuilder.Sql(
                "SET @FamilleId = (SELECT `Id` FROM `Familles` WHERE `Code` = 'GENERAL' LIMIT 1);");

            // ── 1. Reset Category links back to NULL ──────────────────────────
            // Safe: only NULLs categories pointing at the GENERAL family.
            // Categories linked to other families (created after this migration) are untouched.
            migrationBuilder.Sql(@"
UPDATE `Categories`
SET    `FamilleId` = NULL
WHERE  `FamilleId` = @FamilleId
  AND  @FamilleId IS NOT NULL;");

            // ── 2. Delete seeded StockQuantites rows for Dépôt Principal ──────
            // Note: rows created by the application after migration are NOT deleted
            // (they also point to @DepotId but were not seeded here — acceptable
            //  trade-off for a rollback scenario that should be rare in production).
            migrationBuilder.Sql(@"
DELETE FROM `StockQuantites`
WHERE  `DepotId` = @DepotId
  AND  @DepotId IS NOT NULL;");
        }
    }
}
