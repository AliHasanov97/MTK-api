using Microsoft.EntityFrameworkCore;
using MTK.Modules.Hr.Application.Abstractions.Organization;
using MTK.Modules.Hr.Domain.Companies;
using MTK.Modules.Hr.Infrastructure.Database;

namespace MTK.Modules.Hr.Infrastructure.Organization;

/// <summary>
/// Şirkət rekvizitləri — Payments-dən ("mühasibatlıq" tərəfi, həqiqət mənbəyi) sinxronlaşan
/// Company snapshot-undan oxunur (əvvəllər appsettings "Hr:Organization"-dan statik oxunurdu).
/// Scoped ömür boyu bir dəfə sorğulanır (bu request üçün dəyişmir); Company hələ
/// sinxronlaşmayıbsa (ilk deploy, event hələ çatmayıb) köhnə statik default-lara keçir ki,
/// 20+ export servisi boş/null ad ilə qalmasın.
/// </summary>
internal sealed class OrganizationInfo : IOrganizationInfo
{
    private readonly HrDbContext _context;
    private Company? _company;
    private bool _loaded;

    public OrganizationInfo(HrDbContext context)
    {
        _context = context;
    }

    private Company? Current
    {
        get
        {
            if (!_loaded)
            {
                _company = _context.Set<Company>().AsNoTracking().FirstOrDefault();
                _loaded = true;
            }

            return _company;
        }
    }

    public string Name => Current?.Name ?? "MTK";

    public string? Director => Current?.Director;

    public string Address => Current?.Address ?? "Bakı şəhəri";
}
