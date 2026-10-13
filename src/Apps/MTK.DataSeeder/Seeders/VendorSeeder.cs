using Bogus;
using MediatR;
using Microsoft.Extensions.Logging;
using MTK.Modules.Payments.Application.Vendors.Commands.CreateVendor;
using MTK.Modules.Payments.Domain.Repositories;
using MTK.Modules.Payments.Domain.Vendors;

namespace MTK.DataSeeder.Seeders;

/// <summary>
/// Tədarükçü (vendor) seed edir
/// </summary>
public sealed class VendorSeeder
{
    private readonly ISender _sender;
    private readonly ILogger<VendorSeeder> _logger;
    private readonly IVendorRepository _vendorRepository;

    public static readonly List<Guid> VendorIds = new();

    public VendorSeeder(ISender sender, ILogger<VendorSeeder> logger, IVendorRepository vendorRepository)
    {
        _sender = sender;
        _logger = logger;
        _vendorRepository = vendorRepository;
    }

    public async Task SeedAsync()
    {
        // Əvvəlcə bütün mövcud tədarükçüləri bazadan oxu
        var existingVendors = await _vendorRepository.ListAsync();
        VendorIds.Clear();
        VendorIds.AddRange(existingVendors.Select(v => v.Id));

        if (VendorIds.Any())
        {
            _logger.LogInformation("  ℹ️  Bazada artıq {Count} tədarükçü mövcuddur", VendorIds.Count);
            return;
        }

        var faker = new Faker("tr"); // Türk dili (Bogus az_AZ dəstəkləmir)

        // Müxtəlif növ tədarükçülər yaradaq
        var vendors = new List<(string Name, VendorType Type, string? Voen, string? Director, string? Email, string? Phone, string? Address, string? Note)>
        {
            // Hüquqi şəxslər (Legal Entities)
            ("Lift Texniki Xidmət MMC", VendorType.LegalEntity, "1234567890", "Əli Məmmədov", "info@lifttexniki.az", "+994-12-555-11-22", "Bakı şəh., Nəsimi ray., 28 May küç. 10", "Lift texniki xidməti və təmiri"),
            ("Qaz Təchizatı ASC", VendorType.LegalEntity, "2345678901", "Vüsal Həsənov", "contact@qaztechizat.az", "+994-12-555-22-33", "Bakı şəh., Xətai ray., Atatürk pr. 125", "Qaz təchizatı və ödəniş sistemləri"),
            ("Su Kanalı MMC", VendorType.LegalEntity, "3456789012", "Fərid Əliyev", "info@sukanali.az", "+994-12-555-33-44", "Bakı şəh., Sabunçu ray., Bakıxanov qəs. 5", "Su təchizatı və kanalizasiya"),
            ("Elektrik Şəbəkəsi MMC", VendorType.LegalEntity, "4567890123", "Rəşad Quliyev", "info@elektrikshebeke.az", "+994-12-555-44-55", "Bakı şəh., Nərimanov ray., Təbriz küç. 15", "Elektrik enerjisi təchizatı"),
            ("Təmizlik Xidmətləri MMC", VendorType.LegalEntity, "5678901234", "Səbinə Cəfərova", "temizlik@temizlik.az", "+994-12-555-55-66", "Bakı şəh., Binəqədi ray., Azadlıq pr. 88", "Binanın təmizlənməsi və sanitariya"),
            ("Mühafizə Şirkəti ASC", VendorType.LegalEntity, "6789012345", "Kamran Məmmədov", "security@muhafize.az", "+994-12-555-66-77", "Bakı şəh., Yasamal ray., M. Müşfiq küç. 28", "Təhlükəsizlik və mühafizə xidmətləri"),
            ("Landşaft Dizayn MMC", VendorType.LegalEntity, "7890123456", "Günel Həsənova", "info@landshaft.az", "+994-12-555-77-88", "Bakı şəh., Səbail ray., Nizami küç. 45", "Yaşıllıq və bağ-bağça xidməti"),

            // Fərdi sahibkarlar (Sole Proprietors)
            ("Elektrik Təmiri FS", VendorType.SoleProprietor, "8901234567", "Elçin Bayramov", "elchin@elektriktamir.az", "+994-50-555-88-99", "Bakı şəh., Suraxanı ray., Hövsan qəs. 12", "Elektrik sistemlərinin təmiri"),
            ("Santexnika Ustası FS", VendorType.SoleProprietor, "9012345678", "Tural Qasımov", "tural@santexnika.az", "+994-55-555-99-00", "Bakı şəh., Qaradağ ray., Lökbatan qəs. 8", "Santexnika işləri və təmir"),

            // Fiziki şəxslər (Individuals)
            ("İsmayıl Məmmədov", VendorType.Individual, null, null, "ismayil@example.az", "+994-70-555-00-11", "Bakı şəh., Xəzər ray., Mərdəkan qəs. 22", "Kiçik təmir işləri"),
        };

        foreach (var vendor in vendors)
        {
            var command = new CreateVendorCommand(
                vendor.Name,
                vendor.Type,
                vendor.Voen,
                vendor.Director,
                vendor.Email,
                vendor.Phone,
                vendor.Address,
                vendor.Note);

            var result = await _sender.Send(command);

            if (result.IsSuccess)
            {
                VendorIds.Add(result.Value);
                _logger.LogInformation("  ✅ Tədarükçü yaradıldı: {Name} ({Type})", vendor.Name, vendor.Type);
            }
            else
            {
                _logger.LogError("  ❌ Tədarükçü yaradıla bilmədi: {Error}", result.Error);
            }
        }

        _logger.LogInformation("  📊 Cəmi {Count} tədarükçü yaradıldı", VendorIds.Count);
    }
}