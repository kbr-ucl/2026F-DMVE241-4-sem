using Inventory.Facade.DTOs;
namespace Inventory.Facade.Interfaces;
public interface IStockQueries { Task<IReadOnlyList<TicketStockDto>> GetByEventAsync(GetStockRequest request); }