using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Application.OwnerBalances.Queries.GetOwnerBalance;
using MTK.Modules.Payments.Application.OwnerBalances.Services;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.OwnerBalances.Commands.RecalculateOwnerBalance;

internal sealed class RecalculateOwnerBalanceCommandHandler(
    IOwnerBalanceService ownerBalanceService,
    IOwnerBalanceRepository ownerBalanceRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RecalculateOwnerBalanceCommand, OwnerBalanceResponse>
{
    public async Task<Result<OwnerBalanceResponse>> Handle(
        RecalculateOwnerBalanceCommand request,
        CancellationToken cancellationToken)
    {
        await ownerBalanceService.RecalculateAsync([request.OwnerId], cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        var balance = await ownerBalanceRepository.GetByOwnerIdAsync(request.OwnerId, cancellationToken);

        if (balance is null)
        {
            return Result.Failure<OwnerBalanceResponse>(
                new Error("OwnerBalance.NotFound", $"Sahib balansı tapılmadı: {request.OwnerId}"));
        }

        return Result.Success(new OwnerBalanceResponse(
            balance.Id,
            balance.OwnerId,
            balance.TotalDebt,
            balance.TotalPaid,
            balance.CurrentBalance));
    }
}
