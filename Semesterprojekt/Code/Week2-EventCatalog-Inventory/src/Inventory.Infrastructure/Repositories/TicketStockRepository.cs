using Inventory.Domain.Entities;
using Inventory.Infrastructure.Persistence;
using Inventory.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Repositories;
public class TicketStockRepository : ITicketStockRepository
{
    private readonly InventoryDbContext _db;
    public TicketStockRepository(InventoryDbContext db) => _db = db;
    public async Task<TicketStock?> GetByCategoryIdAsync(Guid id) => await _db.TicketStocks.FirstOrDefaultAsync(s => s.CategoryId == id);
    public async Task<IReadOnlyList<TicketStock>> GetByEventIdAsync(Guid eventId) => await _db.TicketStocks.Where(s => s.EventId == eventId).ToListAsync();
    public async Task AddAsync(TicketStock stock) => await _db.TicketStocks.AddAsync(stock);
    public async Task SaveAsync() => await _db.SaveChangesAsync();
}
