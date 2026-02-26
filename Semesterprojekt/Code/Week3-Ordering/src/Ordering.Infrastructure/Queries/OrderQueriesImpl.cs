using Microsoft.EntityFrameworkCore;
using Ordering.Facade.DTOs;
using Ordering.Facade.Interfaces;
using Ordering.Infrastructure.Persistence;
namespace Ordering.Infrastructure.Queries;
public class OrderQueriesImpl : IOrderQueries
{
    private readonly OrderingDbContext _db;
    public OrderQueriesImpl(OrderingDbContext db) => _db = db;
    public async Task<OrderDto?> GetByIdAsync(GetOrderRequest request)
        => await _db.Orders.AsNoTracking().Include(o => o.Lines)
            .Where(o => o.Id == request.OrderId)
            .Select(o => new OrderDto(o.Id, o.EventId, o.CustomerEmail.Value, o.Status.ToString(),
                o.TotalAmount.Amount, o.CreatedAt,
                o.Lines.Select(l => new OrderLineDto(l.CategoryId, l.CategoryName, l.Quantity, l.UnitPrice.Amount)).ToList()))
            .FirstOrDefaultAsync();
}
