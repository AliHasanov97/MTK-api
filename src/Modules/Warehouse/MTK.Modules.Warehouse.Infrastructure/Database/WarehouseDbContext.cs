using Microsoft.EntityFrameworkCore;
using MTK.Modules.Warehouse.Application.Abstractions;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Domain.WarehouseTransactions;

namespace MTK.Modules.Warehouse.Infrastructure.Database;

public sealed class WarehouseDbContext : DbContext, IWarehouseDbContext
{
    public WarehouseDbContext(DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public DbSet<Nomenclature> Nomenclatures => Set<Nomenclature>();

    public DbSet<WarehouseTransaction> WarehouseTransactions => Set<WarehouseTransaction>();

    public DbSet<WarehouseStock> WarehouseStock => Set<WarehouseStock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("warehouse");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WarehouseDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
