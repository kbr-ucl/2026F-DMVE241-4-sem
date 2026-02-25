using EventCatalog.Domain.Entities;
namespace EventCatalog.UseCases.Repositories;

/// <summary>
/// Repository-interface i Use Case-laget jf. Clean Architecture.
/// Implementeres i Infrastructure.
/// </summary>
public interface IEventRepository
{
    Task<Event?> GetByIdAsync(Guid id);
    Task AddAsync(Event evt);
    Task SaveAsync();
}
