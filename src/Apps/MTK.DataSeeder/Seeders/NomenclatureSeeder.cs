using Microsoft.Extensions.Logging;
using MTK.Modules.Warehouse.Domain.Nomenclatures;
using MTK.Modules.Warehouse.Infrastructure.Database;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Nomenklatura (warehouse items) seed edir
/// </summary>
public sealed class NomenclatureSeeder
{
    private readonly ILogger<NomenclatureSeeder> _logger;
    private readonly INomenclatureRepository _nomenclatureRepository;
    private readonly WarehouseDbContext _dbContext;

    public static readonly List<Guid> NomenclatureIds = new();

    public NomenclatureSeeder(
        ILogger<NomenclatureSeeder> logger,
        INomenclatureRepository nomenclatureRepository,
        WarehouseDbContext dbContext)
    {
        _logger = logger;
        _nomenclatureRepository = nomenclatureRepository;
        _dbContext = dbContext;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud nomenklaturları bazadan oxu
        var existingNomenclatures = await _nomenclatureRepository.ListAsync();
        NomenclatureIds.Clear();
        NomenclatureIds.AddRange(existingNomenclatures.Select(n => n.Id));

        if (NomenclatureIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} nomenklatura mövcuddur", NomenclatureIds.Count);
            return;
        }

        // Müxtəlif nomenklaturalar
        var nomenclatures = new List<(string Code, string Name, string? Description, NomenclatureCategory Category, Unit Unit, decimal? MinStockLevel)>
        {
            // Materiallar
            ("MAT-001", "Elektrik kabeli 3x2.5", "Bakır elektrik kabeli, 3 damarlı, 2.5mm²", NomenclatureCategory.Material, Unit.Meter, 100),
            ("MAT-002", "Elektrik kabeli 3x1.5", "Bakır elektrik kabeli, 3 damarlı, 1.5mm²", NomenclatureCategory.Material, Unit.Meter, 100),
            ("MAT-003", "Polipropilen boru 20mm", "Su təchizatı üçün plastik boru", NomenclatureCategory.Material, Unit.Meter, 50),
            ("MAT-004", "Polipropilen boru 25mm", "Su təchizatı üçün plastik boru", NomenclatureCategory.Material, Unit.Meter, 50),
            ("MAT-005", "Sement", "Portland sement", NomenclatureCategory.Material, Unit.Sack, 20),
            ("MAT-006", "Rəng (ağ)", "Ağ divar rəngi", NomenclatureCategory.Material, Unit.Liter, 10),
            ("MAT-007", "Kafel", "Divar üçün kafel", NomenclatureCategory.Material, Unit.SquareMeter, 20),

            // Avadanlıqlar
            ("EQP-001", "Avtomatik açar 16A", "1-fazalı avtomatik qoruyucu açar", NomenclatureCategory.Equipment, Unit.Piece, 10),
            ("EQP-002", "Avtomatik açar 25A", "1-fazalı avtomatik qoruyucu açar", NomenclatureCategory.Equipment, Unit.Piece, 10),
            ("EQP-003", "LED lampa 10W", "Enerji qənaətli LED işıqlandırma", NomenclatureCategory.Equipment, Unit.Piece, 20),
            ("EQP-004", "LED lampa 15W", "Enerji qənaətli LED işıqlandırma", NomenclatureCategory.Equipment, Unit.Piece, 20),
            ("EQP-005", "Rozetka", "Avropa tipli elektrik rozetkası", NomenclatureCategory.Equipment, Unit.Piece, 15),
            ("EQP-006", "Açar (işıq)", "Elektrik işıq açarı", NomenclatureCategory.Equipment, Unit.Piece, 15),
            ("EQP-007", "Qapı dəstəyi", "Daxili qapı üçün qol dəstəyi", NomenclatureCategory.Equipment, Unit.Set, 5),

            // Təchizat
            ("SUP-001", "Təmizlik deterjanı", "Ümumi təmizlik üçün maye təmizləyici", NomenclatureCategory.Supply, Unit.Liter, 5),
            ("SUP-002", "Zibil torbaları", "120L tutumlu zibil torbaları", NomenclatureCategory.Supply, Unit.Package, 10),
            ("SUP-003", "Əlcəklər (rezin)", "Təmizlik üçün rezin əlcəklər", NomenclatureCategory.Supply, Unit.Piece, 20),
            ("SUP-004", "Kağız dəsmal", "Ümumi istifadə üçün kağız dəsmal", NomenclatureCategory.Supply, Unit.Package, 10),
            ("SUP-005", "Mop", "Döşəmə təmizləmə mopu", NomenclatureCategory.Supply, Unit.Piece, 5),

            // Xidmətlər
            ("SRV-001", "Lift təmiri (1 saat)", "Texniki personalın iş saatı", NomenclatureCategory.Service, Unit.Piece, null),
            ("SRV-002", "Elektrik təmiri (1 saat)", "Elektrik ustasının iş saatı", NomenclatureCategory.Service, Unit.Piece, null),
            ("SRV-003", "Santexnik işi (1 saat)", "Santexnik ustanın iş saatı", NomenclatureCategory.Service, Unit.Piece, null),
        };

        foreach (var item in nomenclatures)
        {
            var nomenclature = Nomenclature.Create(
                item.Code,
                item.Name,
                item.Description,
                item.Category,
                item.Unit,
                item.MinStockLevel,
                true);

            _nomenclatureRepository.Add(nomenclature);
            NomenclatureIds.Add(nomenclature.Id);

            _logger.LogInformation("  ✅ Nomenklatura yaradıldı: {Code} - {Name}", item.Code, item.Name);
        }

        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("  📊 Cəmi {Count} nomenklatura yaradıldı", NomenclatureIds.Count);
    }
}