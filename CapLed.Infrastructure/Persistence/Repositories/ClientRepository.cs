using Microsoft.EntityFrameworkCore;
using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities.Commercial;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly StockManagementDbContext _ctx;
    public ClientRepository(StockManagementDbContext ctx) => _ctx = ctx;

    public Task<Client?> GetByIdAsync(int id)    => _ctx.Clients.FindAsync(id).AsTask();
    public Task<Client?> GetByEmailAsync(string email) => _ctx.Clients.FirstOrDefaultAsync(c => c.Email == email);
    public Task<List<Client>> GetAllAsync()      => _ctx.Clients.OrderBy(c => c.Nom).ToListAsync();

    public async Task AddAsync(Client client)    => await _ctx.Clients.AddAsync(client);
    public Task UpdateAsync(Client client)       { _ctx.Clients.Update(client); return Task.CompletedTask; }
}
