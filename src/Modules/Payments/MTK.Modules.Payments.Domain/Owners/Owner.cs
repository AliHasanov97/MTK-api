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
    private Owner(Guid id, string fullName, string email) : base(id)
    {
        FullName = fullName;
        Email = email;
    }

    private Owner() : base() { }

    public string FullName { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public static Owner Create(Guid id, string fullName, string email)
    {
        var owner = new Owner(id, fullName, email);
        owner.SetCreatedAt();
        return owner;
    }

    public void UpdateContactInfo(string fullName, string email)
    {
        FullName = fullName;
        Email = email;
        SetUpdatedAt();
    }
}
