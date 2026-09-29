using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.OwnerBalances.Queries.GetPropertyBalance;

internal sealed class GetPropertyBalanceQueryHandler : IQueryHandler<GetPropertyBalanceQuery, PropertyBalanceResponse>
{
    private readonly IChargeRepository _chargeRepository;

    public GetPropertyBalanceQueryHandler(IChargeRepository chargeRepository)
    {
        _chargeRepository = chargeRepository;
    }

    public async Task<Result<PropertyBalanceResponse>> Handle(
        GetPropertyBalanceQuery request,
        CancellationToken cancellationToken)
    {
        var charges = await _chargeRepository.GetByPropertyIdAsync(request.PropertyId, cancellationToken);

        decimal totalDebt = charges.Sum(c => c.Amount);
        decimal totalPaid = charges.Sum(c => c.PaidAmount);

        // A single charge can never be paid beyond its own Amount (PaymentAllocationService
        // caps each allocation at the charge's remaining balance), so this is always <= 0.
        // Any surplus payment becomes owner-level advance instead — see GetOwnerBalance and
        // PaymentAllocationService.SettleChargeFromAdvanceAsync, which is what actually spends it.
        var response = new PropertyBalanceResponse(
            request.PropertyId,
            totalDebt,
            totalPaid,
            totalPaid - totalDebt);

        return response;
    }
}
