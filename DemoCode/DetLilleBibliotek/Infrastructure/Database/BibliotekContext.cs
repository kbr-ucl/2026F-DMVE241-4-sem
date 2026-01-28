using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database
{
    public class BibliotekContext : DbContext
    {
        public DbSet<Bog> Bøger { get; set; }
        public DbSet<Medlem> Medlemmer { get; set; }

        public BibliotekContext(DbContextOptions<BibliotekContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bog>(entity =>
            {
                entity.HasKey(b => b.Isbn);
            });

            modelBuilder.Entity<Medlem>(entity =>
            {
                entity.HasKey(m => m.Medlemsnummer);
                entity.Property(m => m.Medlemsnummer).ValueGeneratedNever();
            });
        }
    }
}
