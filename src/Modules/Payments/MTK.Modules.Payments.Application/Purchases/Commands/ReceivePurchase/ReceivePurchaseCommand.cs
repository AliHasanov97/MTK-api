using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Purchases.Commands.ReceivePurchase;

/// <summary>
/// Alışı qəbul edir — nəticədə Warehouse moduluna stok artımı üçün integration
/// event göndərilir. Verilməzsə, qəbul anı "indi" götürülür.
/// </summary>
public sealed record ReceivePurchaseCommand(
    Guid PurchaseId,
    DateTimeOffset? ReceivedOnUtc = null,
    Guid? ReceivedByUserId = null) : ICommand;
