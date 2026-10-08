using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Purchases.Commands;

namespace MTK.Modules.Payments.Application.Purchases.Commands.UpdatePurchase;

/// <summary>
/// Yalnız hazırlanan (Draft) alış yenilənə bilər. Sətirlər göndərilən siyahı ilə
/// tam əvəz olunur.
/// </summary>
public sealed record UpdatePurchaseCommand(
    Guid PurchaseId,
    DateTimeOffset PurchaseDate,
    List<PurchaseLineInput> Lines,
    string? Note = null) : ICommand;
