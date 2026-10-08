using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Purchases.Commands.CancelPurchase;

/// <summary>Yalnız hələ qəbul edilməmiş alış ləğv edilə bilər.</summary>
public sealed record CancelPurchaseCommand(
    Guid PurchaseId,
    string? Note = null) : ICommand;
