using CatalogService.Models;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Data;

public class CatalogDb : DbContext
{
    public CatalogDb(DbContextOptions<CatalogDb> options) : base(options)
    {
    }

    public DbSet<Article> Articles => Set<Article>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Article>().HasData(
            new Article { Id = Guid.Parse("5adf5875-11e1-4e6d-8e7b-dc6993641d4d"), Name = "Boiler 24kW", Brand = "ThermoLux", WarrantyMonths = 24, Active = true },
            new Article { Id = Guid.Parse("c2c9b6f1-bf3e-4d0c-bf3c-8f210f4e6aa8"), Name = "Mixer Faucet", Brand = "AquaFlow", WarrantyMonths = 12, Active = true },
            new Article { Id = Guid.Parse("3d4d1d90-2d7b-4a72-b4d1-3e1e0ed7d720"), Name = "Radiator Valve", Brand = "HeatPro", WarrantyMonths = 18, Active = true }
        );
    }
}
