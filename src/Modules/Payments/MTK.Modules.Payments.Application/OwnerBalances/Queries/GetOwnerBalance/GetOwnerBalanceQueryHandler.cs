using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;

internal sealed class GetOwnerBalanceQueryHandler : IQueryHandler<GetOwnerBalanceQuery, OwnerBalanceResponse>
{
    private readonly IOwnerBalanceRepository _ownerBalanceRepository;

    public GetOwnerBalanceQueryHandler(IOwnerBalanceRepository ownerBalanceRepository)
    {
        _ownerBalanceRepository = ownerBalanceRepository;
    }

    public async Task<Result<OwnerBalanceResponse>> Handle(GetOwnerBalanceQuery request, CancellationToken cancellationToken)
    {
        var balance = await _ownerBalanceRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        if (balance is null)
        {
            return Result.Failure<OwnerBalanceResponse>(
                new Error("OwnerBalance.NotFound", $"Owner balance not found for owner {request.OwnerId}"));
        }

        var response = new OwnerBalanceResponse(
            balance.Id,
            balance.OwnerId,
            balance.TotalDebt,
            balance.TotalPaid,
            balance.CurrentBalance);

        return Result.Success(response);
    }
}
