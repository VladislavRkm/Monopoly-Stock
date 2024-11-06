using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StockCore.Models;

namespace StockDataAccess;

public class DatabaseContext : DbContext
{
    public DbSet<Box> Boxes { get; set; }
    public DbSet<Pallet> Pallets { get; set; }

    private readonly IConfiguration _configuration;

    public DatabaseContext() { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<Pallet>(entity =>
        {
            entity.HasKey(e => e.PalletId);
        });

        modelBuilder.Entity<Box>(entity =>
        {
            entity.HasKey(e => e.BoxId);
            entity.HasOne(d => d.Pallet)
                  .WithMany(p => p.Boxes)
                  .HasForeignKey(d => d.PalletId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.Property(b => b.PalletId)
        .ValueGeneratedNever();
        });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql("User ID=postgres; Password=postgres; Host=localhost;Port=5432;Database=Stock;");
        }
    }
}
