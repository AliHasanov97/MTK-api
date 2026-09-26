using MTK.Common.Domain.Abstractions;
using MTK.Modules.Buildings.Domain.Apartments;
using MTK.Modules.Buildings.Domain.Owners;
using MTK.Modules.Buildings.Domain.OwnershipHistories.Events;

namespace MTK.Modules.Buildings.Domain.OwnershipHistories;

/// <summary>
/// Mənzil mülkiyyət transfer tarixçəsi
/// Hər dəfə mənzil satılanda bu entity yaradılır
/// </summary>
public sealed class OwnershipHistory : SearchableEntity
{
    private OwnershipHistory(
        Guid id,
        Guid apartmentId,
        Guid? previousOwnerId,
        Guid newOwnerId,
        DateTime transferDate) : base(id)
    {
        ApartmentId = apartmentId;
        PreviousOwnerId = previousOwnerId;
        NewOwnerId = newOwnerId;
        TransferDate = transferDate;
    }

    // Private constructor for EF Core
    private OwnershipHistory() : base()
    {
    }

    public Guid ApartmentId { get; private set; }

    // Köhnə sahib (nullable - ilk dəfə satış zamanı)
    public Guid? PreviousOwnerId { get; private set; }
    public string? PreviousOwnerName { get; private set; }

    // Yeni sahib
    public Guid NewOwnerId { get; private set; }
    public string NewOwnerName { get; private set; } = string.Empty;

    // Transfer məlumatları
    public DateTime TransferDate { get; private set; }
    public decimal? SalePrice { get; private set; }
    public string? Notes { get; private set; }

    // Navigation
    public Apartment Apartment { get; private set; } = null!;
    public Owner? PreviousOwner { get; private set; }
    public Owner NewOwner { get; private set; } = null!;

    public static OwnershipHistory Create(
        Guid apartmentId,
        Guid? previousOwnerId,
        string? previousOwnerName,
        Guid newOwnerId,
        string newOwnerName,
        DateTime transferDate,
        decimal? salePrice = null,
        string? notes = null)
    {
        var history = new OwnershipHistory(
            Guid.NewGuid(),
            apartmentId,
            previousOwnerId,
            newOwnerId,
            transferDate)
        {
            PreviousOwnerName = previousOwnerName,
            NewOwnerName = newOwnerName,
            SalePrice = salePrice,
            Notes = notes
        };

        history.SetCreatedAt();
        history.RaiseDomainEvent(new OwnershipTransferredDomainEvent(
            history.Id,
            apartmentId,
            previousOwnerId,
            newOwnerId,
            transferDate));

        return history;
    }

    /// <summary>
    /// İlk dəfə mənzil satılarkən (əvvəlki sahib yoxdur)
    /// </summary>
    public static OwnershipHistory CreateInitial(
        Guid apartmentId,
        Guid newOwnerId,
        string newOwnerName,
        DateTime transferDate,
        decimal? salePrice = null,
        string? notes = null)
    {
        return Create(
            apartmentId,
            null,
            null,
            newOwnerId,
            newOwnerName,
            transferDate,
            salePrice,
            notes);
    }
}
