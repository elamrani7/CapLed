using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Stock;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class LotRepository : ILotRepository
{
    private readonly StockManagementDbContext _ctx;

    public LotRepository(StockManagementDbContext ctx) => _ctx = ctx;

    public Task<Lot?> GetByNumberAsync(int articleId, int depotId, string numeroLot)
        => _ctx.Lots
            .FirstOrDefaultAsync(l =>
                l.ArticleId == articleId &&
                l.DepotId   == depotId   &&
                l.NumeroLot == numeroLot);

    public async Task AddAsync(Lot lot)
        => await _ctx.Lots.AddAsync(lot);

    public Task UpdateAsync(Lot lot)
    {
        _ctx.Lots.Update(lot);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Lot lot)
    {
        _ctx.Lots.Remove(lot);
        return Task.CompletedTask;
    }
}
