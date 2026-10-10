using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Application.EducationLeaveOrders.DeleteEducationLeaveOrder;

internal sealed class DeleteEducationLeaveOrderCommandHandler
    : ICommandHandler<DeleteEducationLeaveOrderCommand>
{
    private readonly IEducationLeaveOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEducationLeaveOrderCommandHandler(
        IEducationLeaveOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        DeleteEducationLeaveOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);

        if (order == null)
            return Result.Failure(EducationLeaveOrderErrors.NotFound);

        await _repository.DeleteAsync(order.Id, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
