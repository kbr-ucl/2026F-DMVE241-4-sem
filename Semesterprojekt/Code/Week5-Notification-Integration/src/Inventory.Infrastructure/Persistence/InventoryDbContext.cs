using Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Inventory.Infrastructure.Persistence;
public class InventoryDbContext : DbContext
{
    public DbSet<TicketStock> TicketStocks => Set<TicketStock>();
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder mb)
    { mb.Entity<TicketStock>(e => { e.HasIndex(x => x.CategoryId).IsUnique(); }); }
}
