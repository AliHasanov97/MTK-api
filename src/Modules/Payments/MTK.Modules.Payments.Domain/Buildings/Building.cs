using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Buildings;

/// <summary>
/// Buildings-dəki Building-in bu moduldakı adı-snapshot-u (Id = Buildings Building.Id).
/// Yalnız "bu bina hardadır?" sualına modullar arası DB join olmadan cavab vermək üçün
/// (məs. ödəniş qəbzinin "... ünvanında yerləşən ... binasında" sətri) — Buildings-in
/// Apartment integration event-ləri ilə (bina adı/ünvanı onların daxilində gəlir)
/// sinxronlaşır, ayrıca Building event-i lazım deyil.
/// </summary>
public sealed class Building : Entity
{
    private Building(Guid id, string name, string address) : base(id)
    {
        Name = name;
        Address = address;
    }

    private Building() : base() { }

    public string Name { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;

    public static Building Create(Guid id, string name, string address)
    {
        var building = new Building(id, name, address);
        building.SetCreatedAt();
        return building;
    }

    public void Update(string name, string address)
    {
        Name = name;
        Address = address;
        SetUpdatedAt();
    }
}
