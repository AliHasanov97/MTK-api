using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Owners.Commands.CreatePassiveOwner;

/// <summary>
/// User account olmadan Owner yarat
/// Bu owner-lər yalnız məlumat saxlamaq üçündür (invoice, debt tracking)
/// Sistemə daxil ola bilməzlər
/// </summary>
public sealed record CreatePassiveOwnerCommand(
    string FirstName,
    string LastName,
    string PhoneNumber,
    string Email,
    string? Notes = null) : ICommand<Guid>;
