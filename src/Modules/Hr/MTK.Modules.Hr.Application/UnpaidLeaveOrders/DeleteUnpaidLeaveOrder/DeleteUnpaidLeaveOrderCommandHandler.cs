using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Application.UnpaidLeaveOrders.DeleteUnpaidLeaveOrder;

internal sealed class DeleteUnpaidLeaveOrderCommandHandler
    : ICommandHandler<DeleteUnpaidLeaveOrderCommand>
{
    private readonly IUnpaidLeaveOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUnpaidLeaveOrderCommandHandler(
        IUnpaidLeaveOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteUnpaidLeaveOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure(UnpaidLeaveOrderErrors.NotFound);

        await _repository.DeleteAsync(order.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
