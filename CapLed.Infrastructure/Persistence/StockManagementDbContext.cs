using StockManager.Core.Domain.Entities;
using StockManager.Core.Domain.Entities.Catalogue;
using StockManager.Core.Domain.Entities.Stock;
using StockManager.Core.Domain.Entities.Commercial;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace StockManager.Infrastructure.Persistence;

public class StockManagementDbContext : DbContext
{
    public StockManagementDbContext(DbContextOptions<StockManagementDbContext> options)
        : base(options)
    {
    }

    // ── Existing DbSets ───────────────────────────────────────────────────────
    public DbSet<Equipment>      Equipments      => Set<Equipment>();
    public DbSet<Category>       Categories      => Set<Category>();
    public DbSet<ChampSpecifique> ChampsSpecifiques => Set<ChampSpecifique>();
    public DbSet<ArticleChampValeur> ArticleChampValeurs => Set<ArticleChampValeur>();
    public DbSet<ArticleEtatDetail> ArticleEtatDetails => Set<ArticleEtatDetail>();
    public DbSet<StockMovement>  StockMovements  => Set<StockMovement>();
    public DbSet<User>           Users           => Set<User>();
    public DbSet<Photo>          Photos          => Set<Photo>();
    public DbSet<ContactRequest> ContactRequests => Set<ContactRequest>();

    // ── Step 1B: Core ERP Stock DbSets ────────────────────────────────────────
    public DbSet<Famille>      Familles      => Set<Famille>();
    public DbSet<Depot>        Depots        => Set<Depot>();
    public DbSet<StockQuantite> StockQuantites => Set<StockQuantite>();
    public DbSet<AlerteStock>  AlertesStock  => Set<AlerteStock>();
    public DbSet<Lot>           Lots           => Set<Lot>();
    public DbSet<NumeroSerie>   NumerosSerie   => Set<NumeroSerie>();

    // ── Step 4A: Commercial module DbSets ─────────────────────────────────────
    public DbSet<Client>     Clients  => Set<Client>();
    public DbSet<Lead>       Leads    => Set<Lead>();
    public DbSet<LigneLead>  LignesLead => Set<LigneLead>();

    // ── Step 4B: Orders & Deliveries (BC/BL) ──────────────────────────────────
    public DbSet<BonCommande> BonsCommande => Set<BonCommande>();
    public DbSet<LigneBC>     LignesBC     => Set<LigneBC>();
    public DbSet<BonLivraison> BonsLivraison => Set<BonLivraison>();
    public DbSet<LigneBL>     LignesBL     => Set<LigneBL>();
    // ─────────────────────────────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all configurations from the current assembly
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
