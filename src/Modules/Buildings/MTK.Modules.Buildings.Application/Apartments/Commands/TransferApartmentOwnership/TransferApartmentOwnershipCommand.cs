using MTK.Common.Application.Messaging;
using System.Text.Json.Serialization;

namespace MTK.Modules.Buildings.Application.Apartments.Commands.TransferApartmentOwnership;

/// <summary>
/// Mənzilin mülkiyyətini transfer etmək üçün command
/// Business rules:
/// 1. Köhnə owner-in borcu 0 olmalıdır (Billing module yoxlayacaq)
/// 2. Transfer həmişə ayın 1-dən başlayır
/// 3. OwnershipHistory yaradılır
/// 4. Integration event publish edilir
///
/// No client-supplied TransferDate/SalePrice/Notes: a transfer always takes
/// effect now (the handler stamps it with DateTime.UtcNow, then snaps to the
/// 1st of that month as before) — sale price and notes aren't tracked here.
/// </summary>
public sealed class TransferApartmentOwnershipCommand : ICommand
{
    public Guid NewOwnerId { get; set; }

    [JsonIgnore]
    public Guid ApartmentId { get; set; }
}
