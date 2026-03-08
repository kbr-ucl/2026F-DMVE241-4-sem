using Ordering.Facade.DTOs;
namespace Ordering.Facade.Interfaces;
public interface IOrderQueries { Task<OrderDto?> GetByIdAsync(GetOrderRequest request); }