using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Application.OrdersForChangeOfPosition.DeleteOrderForChangeOfPosition;

internal sealed class DeleteOrderForChangeOfPositionCommandHandler
    : ICommandHandler<DeleteOrderForChangeOfPositionCommand>
{
    private readonly IOrderForChangeOfPositionRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteOrderForChangeOfPositionCommandHandler(
        IOrderForChangeOfPositionRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteOrderForChangeOfPositionCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (order is null)
            return Result.Failure(OrderForChangeOfPositionErrors.NotFound);

        await _repository.DeleteAsync(order, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
