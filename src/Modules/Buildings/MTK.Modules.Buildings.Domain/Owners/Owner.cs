using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Owners.Events;

namespace MTK.Modules.Buildings.Domain.Owners;

/// <summary>
/// Mənzil sahibi entity - Aggregate Root
/// Bu entity Identity module-dəki User entity ilə əlaqəlidir
/// </summary>
public sealed class Owner : SearchableEntity
{
    private readonly List<Apartment> _ownedApartments = new();
    private readonly List<Garage> _ownedGarages = new();

    private Owner(
        Guid id,
        Guid? userId,
        string firstName,
        string lastName,
        string phoneNumber,
        string email) : base(id)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        IsActive = true;
    }

    // Private constructor for EF Core
    private Owner() : base()
    {
    }

    // İstifadəçi məlumatları (snapshot - Identity module-dən sync edilir)
    public Guid? UserId { get; private set; }  // Nullable - passive owners don't have user accounts
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";

    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>
    /// İstifadəçi hesabı ilə əlaqələndirilmişmi?
    /// </summary>
    public bool IsRegistered => UserId.HasValue;

    // Navigation
    public IReadOnlyCollection<Apartment> OwnedApartments => _ownedApartments.AsReadOnly();
    public IReadOnlyCollection<Garage> OwnedGarages => _ownedGarages.AsReadOnly();

    public static Owner Create(
        Guid userId,
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string? notes = null)
    {
        var owner = new Owner(
            Guid.NewGuid(),
            userId,
            firstName,
            lastName,
            phoneNumber,
            email)
        {
            Notes = notes
        };

        owner.SetCreatedAt();
        owner.RaiseDomainEvent(new OwnerCreatedDomainEvent(owner.Id, userId, owner.FullName));

        return owner;
    }

    /// <summary>
    /// Passive owner yarat (istifadəçi hesabı olmadan)
    /// Bu owner-lər yalnız məlumat saxlamaq üçündür (invoice, debt tracking)
    /// Sistemi istifadə edə bilməzlər
    /// </summary>
    public static Owner CreatePassive(
        string firstName,
        string lastName,
        string phoneNumber,
        string email,
        string? notes = null)
    {
        var owner = new Owner(
            Guid.NewGuid(),
            null,  // No user account
            firstName,
            lastName,
            phoneNumber,
            email)
        {
            Notes = notes
        };

        owner.SetCreatedAt();
        owner.RaiseDomainEvent(new OwnerCreatedDomainEvent(owner.Id, null, owner.FullName));

        return owner;
    }

    /// <summary>
    /// Passive owner-i User hesabı ilə link et
    /// Owner artıq sistemə daxil ola bilər
    /// </summary>
    public void LinkToUser(Guid userId)
    {
        if (UserId.HasValue)
        {
            throw new InvalidOperationException($"Owner {Id} is already linked to user {UserId}");
        }

        UserId = userId;
        SetUpdatedAt();

        RaiseDomainEvent(new OwnerLinkedToUserDomainEvent(Id, userId));
    }

    public void UpdateContactInfo(
        string firstName,
        string lastName,
        string phoneNumber,
        string email)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Email = email;
        SetUpdatedAt();

        RaiseDomainEvent(new OwnerContactInfoUpdatedDomainEvent(Id, FullName, email, phoneNumber));
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        SetUpdatedAt();
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();

        RaiseDomainEvent(new OwnerActivatedDomainEvent(Id));
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();

        RaiseDomainEvent(new OwnerDeactivatedDomainEvent(Id));
    }

    public void Delete()
    {
        SetDeletedAt();
        RaiseDomainEvent(new OwnerDeletedDomainEvent(Id));
    }

    /// <summary>
    /// Bu owner-in cari olaraq heç bir borcu var?
    /// Ownership transfer zamanı yoxlanılır
    /// </summary>
    public bool HasOutstandingDebt(decimal totalDebt)
    {
        return totalDebt > 0;
    }
}
