using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeOrders.DeleteEmploymentStatusChangeOrder;

internal sealed class DeleteEmploymentStatusChangeOrderCommandHandler
    : ICommandHandler<DeleteEmploymentStatusChangeOrderCommand, DeleteEmploymentStatusChangeOrderResponse>
{
    private readonly IEmploymentStatusChangeOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmploymentStatusChangeOrderCommandHandler(
        IEmploymentStatusChangeOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<DeleteEmploymentStatusChangeOrderResponse>> Handle(
        DeleteEmploymentStatusChangeOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure<DeleteEmploymentStatusChangeOrderResponse>(
                EmploymentStatusChangeOrderErrors.NotFound);

        await _repository.DeleteAsync(order.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new DeleteEmploymentStatusChangeOrderResponse());
    }
}
