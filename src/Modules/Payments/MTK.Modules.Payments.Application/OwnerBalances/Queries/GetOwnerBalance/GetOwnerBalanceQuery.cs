using MTK.Common.Application.Messaging;

namespace MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;

public sealed record GetOwnerBalanceQuery(Guid OwnerId) : IQuery<OwnerBalanceResponse>;
