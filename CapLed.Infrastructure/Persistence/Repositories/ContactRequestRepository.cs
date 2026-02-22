using StockManager.Core.Application.Interfaces.Repositories;
using StockManager.Core.Domain.Entities;
using StockManager.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace StockManager.Infrastructure.Persistence.Repositories;

public class ContactRequestRepository : IContactRequestRepository
{
    private readonly StockManagementDbContext _context;

    public ContactRequestRepository(StockManagementDbContext context)
    {
        _context = context;
    }

    public async Task<ContactRequest?> GetByIdAsync(int id)
    {
        return await _context.ContactRequests
            .Include(r => r.Equipment)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<IEnumerable<ContactRequest>> GetAllAsync()
    {
        return await _context.ContactRequests
            .Include(r => r.Equipment)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task AddAsync(ContactRequest request)
    {
        await _context.ContactRequests.AddAsync(request);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ContactRequest request)
    {
        _context.ContactRequests.Update(request);
        await _context.SaveChangesAsync();
    }
}
