using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Garages;
using MTK.Modules.Buildings.Domain.Owners.Events;

namespace MTK.Modules.Buildings.Domain.Owners;

/// <summary>
/// Mənzil sahibi entity - Aggregate Root
/// Bu entity Identity module-dəki User entity ilə əlaqəlidir
/// </summary>
public sealed class Owner : Entity
{
    private readonly List<Apartment> _ownedApartments = new();
    private readonly List<Garage> _ownedGarages = new();

    private Owner(
        Guid id,
        Guid userId,
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
    private Owner() : base(Guid.Empty)
    {
    }

    // İstifadəçi məlumatları (snapshot - Identity module-dən sync edilir)
    public Guid UserId { get; private set; }
    public string FirstName { get; private set; } = string.Empty;
    public string LastName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";

    public bool IsActive { get; private set; }
    public string? Notes { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public DateTime? DeletedAt { get; private set; }

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
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };

        owner.RaiseDomainEvent(new OwnerCreatedDomainEvent(owner.Id, userId, owner.FullName));

        return owner;
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
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OwnerContactInfoUpdatedDomainEvent(Id, FullName, email, phoneNumber));
    }

    public void UpdateNotes(string? notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OwnerActivatedDomainEvent(Id));
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new OwnerDeactivatedDomainEvent(Id));
    }

    public void Delete()
    {
        DeletedAt = DateTime.UtcNow;
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
