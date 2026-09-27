using MTK.Common.Application.Messaging;

namespace MTK.Modules.Buildings.Application.Owners.Commands.LinkOwnerToUser;

/// <summary>
/// Passiv sahibi (hesabı olmayan) mövcud bir istifadəçi hesabına bağlayır.
/// Yalnız UserId-si hələ təyin olunmamış sahiblər üçün işləyir.
/// </summary>
public sealed record LinkOwnerToUserCommand(Guid OwnerId, Guid UserId) : ICommand;
