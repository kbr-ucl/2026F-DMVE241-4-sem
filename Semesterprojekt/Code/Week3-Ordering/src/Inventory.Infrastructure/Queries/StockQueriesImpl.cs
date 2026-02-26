using Inventory.Facade.DTOs;
using Inventory.Facade.Interfaces;
using Inventory.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Queries;
public class StockQueriesImpl : IStockQueries
{
    private readonly InventoryDbContext _db;
    public StockQueriesImpl(InventoryDbContext db) => _db = db;
    public async Task<IReadOnlyList<TicketStockDto>> GetByEventAsync(GetStockRequest request)
        => await _db.TicketStocks.AsNoTracking().Where(s => s.EventId == request.EventId)
            .Select(s => new TicketStockDto(s.CategoryId, s.CategoryName, s.Available, s.Reserved, s.TotalCapacity)).ToListAsync();
}
