using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Vendors.Events;

namespace MTK.Modules.Payments.Domain.Vendors;

/// <summary>
/// Mal/xidmət tədarükçüsü (Aggregate Root). Ödənişlər modulunda saxlanılır, çünki
/// onunla bağlanan müqavilələr və hesablaşma axını bu modulun məsuliyyətidir.
/// Bank hesabları (VendorBankAccount) hələlik yoxdur — əlavə olunanda yalnız
/// <see cref="Id"/> üzərindən bağlanacaq.
/// </summary>
public sealed class Vendor : SearchableEntity
{
    private Vendor() : base() { }

    public string Name { get; private set; } = string.Empty;

    public VendorType VendorType { get; private set; }

    /// <summary>Vergi ödəyicisinin eyniləşdirmə nömrəsi (VÖEN).</summary>
    public string? Voen { get; private set; }

    public string? Director { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? Note { get; private set; }

    public bool IsActive { get; private set; }

    
    public static Vendor Create(
        string name,
        VendorType vendorType,
        string? voen = null,
        string? director = null,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? note = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vendor adı mütləqdir", nameof(name));

        var vendor = new Vendor
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            VendorType = vendorType,
            Voen = voen,
            Director = director,
            Email = email,
            Phone = phone,
            Address = address,
            Note = note,
            IsActive = true,
        };

        vendor.SetCreatedAt();
        vendor.RaiseDomainEvent(new VendorCreatedDomainEvent(vendor.Id, vendor.Name, vendorType));

        return vendor;
    }

    public void UpdateIdentity(string name, VendorType vendorType, string? voen)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Vendor adı mütləqdir", nameof(name));

        Name = name.Trim();
        VendorType = vendorType;
        Voen = voen;
        SetUpdatedAt();

        RaiseDomainEvent(new VendorUpdatedDomainEvent(Id, Name));
    }

    public void UpdateContactInfo(
        string? director,
        string? email,
        string? phone,
        string? address,
        string? note)
    {
        Director = director;
        Email = email;
        Phone = phone;
        Address = address;
        Note = note;
        SetUpdatedAt();

        RaiseDomainEvent(new VendorUpdatedDomainEvent(Id, Name));
    }
    
    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();

        RaiseDomainEvent(new VendorDeactivatedDomainEvent(Id));
    }

    /// <summary>
    /// Soft delete. Aktiv müqaviləsi olan tədarükçünü silmək əvəzinə
    /// <see cref="Deactivate"/> çağırılmalıdır — yoxlama command handler-də olur.
    /// </summary>
    public void Delete()
    {
        SetDeletedAt();
    }
}
