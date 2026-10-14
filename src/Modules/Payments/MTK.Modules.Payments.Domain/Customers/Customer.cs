using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Customers.Events;

namespace MTK.Modules.Payments.Domain.Customers;

/// <summary>
/// Müştəri (Aggregate Root) — bizə ödəniş edən kənar tərəf (məs. kommersiya sahəsini
/// icarəyə götürən). <see cref="Vendors.Vendor"/>-un güzgüsüdür: tədarükçüyə biz borcluyuq,
/// müştəri isə bizə borcludur.
/// </summary>
public sealed class Customer : SearchableEntity
{
    private Customer() : base() { }

    public string Name { get; private set; } = string.Empty;

    public CustomerType CustomerType { get; private set; }

    /// <summary>Vergi ödəyicisinin eyniləşdirmə nömrəsi (VÖEN).</summary>
    public string? Voen { get; private set; }

    public string? Director { get; private set; }
    public string? Email { get; private set; }
    public string? Phone { get; private set; }
    public string? Address { get; private set; }
    public string? Note { get; private set; }

    public bool IsActive { get; private set; }

    public static Customer Create(
        string name,
        CustomerType customerType,
        string? voen = null,
        string? director = null,
        string? email = null,
        string? phone = null,
        string? address = null,
        string? note = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Müştərinin adı mütləqdir", nameof(name));

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            CustomerType = customerType,
            Voen = voen,
            Director = director,
            Email = email,
            Phone = phone,
            Address = address,
            Note = note,
            IsActive = true,
        };

        customer.SetCreatedAt();
        customer.RaiseDomainEvent(new CustomerCreatedDomainEvent(customer.Id, customer.Name, customerType));

        return customer;
    }

    public void UpdateIdentity(string name, CustomerType customerType, string? voen)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Müştərinin adı mütləqdir", nameof(name));

        Name = name.Trim();
        CustomerType = customerType;
        Voen = voen;
        SetUpdatedAt();

        RaiseDomainEvent(new CustomerUpdatedDomainEvent(Id, Name));
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

        RaiseDomainEvent(new CustomerUpdatedDomainEvent(Id, Name));
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

        RaiseDomainEvent(new CustomerDeactivatedDomainEvent(Id));
    }

    /// <summary>Soft delete. Açıq borcu olan müştərini silmək əvəzinə dayandırmaq tövsiyə olunur.</summary>
    public void Delete()
    {
        SetDeletedAt();
    }
}
