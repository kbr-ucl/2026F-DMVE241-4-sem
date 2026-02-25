using EventCatalog.Domain.Entities;

namespace EventCatalog.UseCases.Repositories;

public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id);
    Task AddAsync(Event evt);
    Task SaveAsync();
}