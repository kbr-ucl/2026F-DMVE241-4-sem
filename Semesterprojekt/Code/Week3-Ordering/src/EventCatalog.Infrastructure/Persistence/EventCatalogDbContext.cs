using EventCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventCatalog.Infrastructure.Persistence;

public class EventCatalogDbContext : DbContext
{
    public DbSet<Event> Events => Set<Event>();
    public DbSet<TicketCategory> TicketCategories => Set<TicketCategory>();

    public EventCatalogDbContext(DbContextOptions<EventCatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>(e =>
        {
            e.Property(x => x.Status).HasConversion<string>();
        });
        modelBuilder.Entity<TicketCategory>(e =>
        {
            e.Property(x => x.Price)
                .HasConversion(new MoneyConverter())
                .HasPrecision(18, 2);
        });
    }
}
