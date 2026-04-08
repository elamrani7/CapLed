using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class NumeroSerieRepository : INumeroSerieRepository
{
    private readonly StockManagementDbContext _ctx;

    public NumeroSerieRepository(StockManagementDbContext ctx) => _ctx = ctx;

    public Task<NumeroSerie?> GetBySerialAsync(string numeroSerie)
        => _ctx.NumerosSerie
            .FirstOrDefaultAsync(ns => ns.NumeroSerieLabel == numeroSerie);

    public Task<List<NumeroSerie>> GetBySerialListAsync(List<string> serials, int depotId)
        => _ctx.NumerosSerie
            .Where(ns => serials.Contains(ns.NumeroSerieLabel) && ns.DepotId == depotId)
            .ToListAsync();

    public async Task AddRangeAsync(IEnumerable<NumeroSerie> series)
        => await _ctx.NumerosSerie.AddRangeAsync(series);

    public Task UpdateRangeAsync(IEnumerable<NumeroSerie> series)
    {
        _ctx.NumerosSerie.UpdateRange(series);
        return Task.CompletedTask;
    }
}
