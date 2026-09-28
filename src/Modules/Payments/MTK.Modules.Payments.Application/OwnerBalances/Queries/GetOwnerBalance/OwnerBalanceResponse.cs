namespace MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;

public sealed record OwnerBalanceResponse(
    Guid Id,
    Guid OwnerId,
    decimal TotalDebt,
    decimal TotalPaid,
    decimal CurrentBalance);
