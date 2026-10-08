using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Purchases.Commands.DeletePurchase;

/// <summary>Yalnız hazırlanan (Draft) alış silinir.</summary>
public sealed record DeletePurchaseCommand(Guid PurchaseId) : ICommand;
