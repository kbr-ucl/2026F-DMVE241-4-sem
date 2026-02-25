using EventCatalog.Domain.Entities;

namespace EventCatalog.UseCases.Repositories;

public interface ITicketCategoryRepository
{
    Task<TicketCategory?> GetByIdAsync(Guid id);
    Task AddAsync(TicketCategory evt);
    Task SaveAsync();
}