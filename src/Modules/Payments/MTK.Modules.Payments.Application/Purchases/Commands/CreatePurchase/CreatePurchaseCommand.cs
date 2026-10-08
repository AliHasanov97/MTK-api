using MTK.Common.Application.Messaging;
using MTK.Modules.Payments.Application.Purchases.Commands;

namespace MTK.Modules.Payments.Application.Purchases.Commands.CreatePurchase;

public sealed record CreatePurchaseCommand(
    Guid VendorId,
    DateTimeOffset PurchaseDate,
    List<PurchaseLineInput> Lines,
    string? Note = null,
    Guid? CreatedByUserId = null) : ICommand<Guid>;
