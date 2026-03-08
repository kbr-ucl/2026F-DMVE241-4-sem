using Microsoft.EntityFrameworkCore;
using Ordering.Domain.Entities;
namespace Ordering.Infrastructure.Persistence;
public class OrderingDbContext : DbContext
{
    public DbSet<Order> Orders => Set<Order>();
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Order>(e => { e.Property(x => x.Status).HasConversion<string>();
            e.Property(x => x.CustomerEmail).HasConversion(new CustomerEmailConverter());
            e.HasMany(x => x.Lines).WithOne().HasForeignKey("OrderId"); });
        mb.Entity<OrderLine>(e => 
        { 
            e.Property(x => x.UnitPrice)
                .HasConversion(new MoneyConverter())
                .HasPrecision(18,2); 
        });
    }
}
