using Ordering.Domain.Entities;
namespace Ordering.UseCases.Repositories;
public interface IOrderRepository
{ Task<Order?> GetByIdAsync(Guid id); Task AddAsync(Order order); Task SaveAsync(); }
