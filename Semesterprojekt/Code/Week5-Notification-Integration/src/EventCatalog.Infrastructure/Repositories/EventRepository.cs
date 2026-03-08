using EventCatalog.Domain.Entities;
using EventCatalog.Infrastructure.Persistence;
using EventCatalog.UseCases.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EventCatalog.Infrastructure.Repositories;

/// <summary>
/// Bemærk: SaveAsync() kalder IKKE Update(). EF Core tracker
/// ændringer på materialiserede entities automatisk.
/// </summary>
public class EventRepository : IEventRepository
{
    private readonly EventCatalogDbContext _db;
    public EventRepository(EventCatalogDbContext db) => _db = db;

    public async Task<Event?> GetByIdAsync(Guid id)
        => await _db.Events.Include(e => e.TicketCategories).FirstOrDefaultAsync(e => e.Id == id);
    public async Task AddAsync(Event evt) => await _db.Events.AddAsync(evt);
    public async Task SaveAsync() => await _db.SaveChangesAsync();
}
