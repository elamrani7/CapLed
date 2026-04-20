using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class LeadRepository : ILeadRepository
{
    private readonly StockManagementDbContext _ctx;
    public LeadRepository(StockManagementDbContext ctx) => _ctx = ctx;

    public Task<Lead?> GetByIdAsync(int id)
        => _ctx.Leads
            .Include(l => l.Client)
            .Include(l => l.Commercial)
            .Include(l => l.Lignes).ThenInclude(lg => lg.Article)
            .FirstOrDefaultAsync(l => l.Id == id);

    public Task<Lead?> GetByNumeroDevisAsync(string numeroDevis)
        => _ctx.Leads.FirstOrDefaultAsync(l => l.NumeroDevis == numeroDevis);

    public Task<List<Lead>> GetAllAsync()
        => _ctx.Leads
            .Include(l => l.Client)
            .Include(l => l.Commercial)
            .Include(l => l.Lignes).ThenInclude(lg => lg.Article)
            .OrderByDescending(l => l.DateSoumission)
            .ToListAsync();

    public Task<List<Lead>> GetByStatutAsync(string statut)
        => _ctx.Leads
            .Include(l => l.Client)
            .Include(l => l.Lignes).ThenInclude(lg => lg.Article)
            .Where(l => l.Statut == statut)
            .OrderByDescending(l => l.DateSoumission)
            .ToListAsync();

    public Task<int> CountByYearAsync(int year)
        => _ctx.Leads.CountAsync(l => l.DateSoumission.Year == year);

    public async Task AddAsync(Lead lead)    => await _ctx.Leads.AddAsync(lead);
    public Task UpdateAsync(Lead lead)       { _ctx.Leads.Update(lead); return Task.CompletedTask; }
}
