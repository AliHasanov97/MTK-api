using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Payments.Application.Abstractions.Data;
using MTK.Modules.Payments.Domain.Repositories;

namespace MTK.Modules.Payments.Application.Purchases.Commands.ReceivePurchase;

internal sealed class ReceivePurchaseCommandHandler : ICommandHandler<ReceivePurchaseCommand>
{
    private readonly IPurchaseRepository _purchaseRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ReceivePurchaseCommandHandler(
        IPurchaseRepository purchaseRepository,
        IUnitOfWork unitOfWork)
    {
        _purchaseRepository = purchaseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(ReceivePurchaseCommand request, CancellationToken cancellationToken)
    {
        var purchase = await _purchaseRepository.GetWithLinesAsync(request.PurchaseId, cancellationToken);
        if (purchase is null)
        {
            return Result.Failure(new Error(
                "Purchase.NotFound",
                $"Alış tapılmadı: {request.PurchaseId}"));
        }

        purchase.Receive(
            request.ReceivedOnUtc ?? DateTimeOffset.UtcNow,
            request.ReceivedByUserId);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
