using Bogus;
using Microsoft.Extensions.Logging;
using MTK.Modules.Warehouse.Domain.WarehouseStock;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Anbar stoku (warehouse stock) seed edir
/// </summary>
public sealed class WarehouseStockSeeder
{
    private readonly ILogger<WarehouseStockSeeder> _logger;
    private readonly IWarehouseStockRepository _warehouseStockRepository;
    private readonly WarehouseDbContext _dbContext;

    public WarehouseStockSeeder(
        ILogger<WarehouseStockSeeder> logger,
        IWarehouseStockRepository warehouseStockRepository,
        WarehouseDbContext dbContext)
    {
        _logger = logger;
        _warehouseStockRepository = warehouseStockRepository;
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        if (!NomenclatureSeeder.NomenclatureIds.Any())
        {
            _logger.LogWarning("  ⚠️  Nomenklaturalar mövcud deyil. Əvvəlcə NomenclatureSeeder işlətməlisiniz.");
            return;
        }

        // Əvvəlcə mövcud stokları yoxla
        var existingStocks = await _warehouseStockRepository.GetAllStockAsync(1, int.MaxValue);
        if (existingStocks.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} stok qeydi mövcuddur", existingStocks.Count());
            return;
        }

        var faker = new Faker();
        var stockCount = 0;

        // Hər nomenklatura üçün random ilkin stok yarat
        foreach (var nomenclatureId in NomenclatureSeeder.NomenclatureIds)
        {
            // Random miqdar (10 ilə 500 arası)
            var initialQuantity = faker.Random.Decimal(10, 500);

            var stock = WarehouseStock.Create(nomenclatureId, initialQuantity);

            _warehouseStockRepository.Add(stock);
            stockCount++;

            if (stockCount % 10 == 0)
            {
                _logger.LogInformation("  ✅ {Count} stok qeydi yaradıldı...", stockCount);
            }
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("  📊 Cəmi {Count} stok qeydi yaradıldı", stockCount);

        // Low stock items göstər
        var lowStockItems = await _warehouseStockRepository.GetLowStockItemsAsync();
        if (lowStockItems.Any())
        {
            _logger.LogInformation("  ⚠️  Minimum səviyyədən aşağı {Count} məhsul var", lowStockItems.Count());
        }
    }
}