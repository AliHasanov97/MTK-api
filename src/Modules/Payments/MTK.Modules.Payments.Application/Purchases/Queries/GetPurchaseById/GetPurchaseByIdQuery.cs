using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Purchases.Queries.GetPurchaseById;

public sealed record GetPurchaseByIdQuery(Guid PurchaseId) : IQuery<PurchaseResponse>;
