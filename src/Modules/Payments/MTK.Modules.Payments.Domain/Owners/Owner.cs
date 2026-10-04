using MTK.Common.Domain.Abstractions;

namespace MTK.Modules.Payments.Domain.Owners;

/// <summary>
/// Buildings-dəki Owner-in bu moduldakı adı-snapshot-u (Id = Buildings Owner.Id).
/// Yalnız "sahib kimdir?" sualına modullar arası DB join olmadan cavab vermək üçün
/// (məs. ödəniş qəbzində "Verilir [ad]") — Buildings-in OwnerCreated/OwnerUpdated
/// integration event-ləri ilə sinxronlaşır. User shadow-dan fərqli olaraq, hər
/// Owner-in hesabı (User) olmur (passiv sahiblər), ona görə bu ayrıca bir snapshot-dur.
/// </summary>
public sealed class Owner : Entity
{
    private Owner(Guid id, string fullName) : base(id)
    {
        FullName = fullName;
    }

    private Owner() : base() { }

    public string FullName { get; private set; } = string.Empty;

    public static Owner Create(Guid id, string fullName)
    {
        var owner = new Owner(id, fullName);
        owner.SetCreatedAt();
        return owner;
    }

    public void UpdateFullName(string fullName)
    {
        FullName = fullName;
        SetUpdatedAt();
    }
}
