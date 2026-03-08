using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;
using Ordering.UseCases.Repositories;
namespace Ordering.Infrastructure.Repositories;
public class OrderRepository : IOrderRepository
{
    private readonly OrderingDbContext _db;
    public OrderRepository(OrderingDbContext db) => _db = db;
    public async Task<Order?> GetByIdAsync(Guid id) => await _db.Orders.Include(o => o.Lines).FirstOrDefaultAsync(o => o.Id == id);
    public async Task AddAsync(Order order) => await _db.Orders.AddAsync(order);
    public async Task SaveAsync() => await _db.SaveChangesAsync();
}
