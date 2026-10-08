using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Commands.CancelPurchase;

internal sealed class CancelPurchaseCommandHandler : ICommandHandler<CancelPurchaseCommand>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelPurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(CancelPurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetByIdAsync(request.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return Result.Failure(new Error(
                "Purchase.NotFound",
                $"Alış tapılmadı: {request.PurchaseId}"));
        }

        purchase.Cancel(request.Note);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
