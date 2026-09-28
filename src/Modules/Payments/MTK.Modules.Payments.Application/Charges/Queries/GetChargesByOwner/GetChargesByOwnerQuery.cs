using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.Charges.Queries.GetChargesByOwner;

public sealed record GetChargesByOwnerQuery(Guid OwnerId) : IQuery<IReadOnlyCollection<ChargeResponse>>;
