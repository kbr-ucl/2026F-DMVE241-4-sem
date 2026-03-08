using Inventory.Domain.Entities;
namespace Inventory.UseCases.Repositories;
public interface ITicketStockRepository
{
    Task<TicketStock?> GetByCategoryIdAsync(Guid categoryId);
    Task<IReadOnlyList<TicketStock>> GetByEventIdAsync(Guid eventId);
    Task AddAsync(TicketStock stock);
    Task SaveAsync();
}
