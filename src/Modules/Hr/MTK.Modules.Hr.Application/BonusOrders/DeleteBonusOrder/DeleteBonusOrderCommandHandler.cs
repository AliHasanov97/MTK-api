using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.BonusOrders;

namespace MTK.Modules.Hr.Application.BonusOrders.DeleteBonusOrder;

internal sealed class DeleteBonusOrderCommandHandler : ICommandHandler<DeleteBonusOrderCommand>
{
    private readonly IBonusOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteBonusOrderCommandHandler(
        IBonusOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteBonusOrderCommand request, CancellationToken cancellationToken)
    {
        var bonusOrder = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (bonusOrder is null)
            return Result.Failure(BonusOrderErrors.NotFound);

        await _repository.DeleteAsync(bonusOrder, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
