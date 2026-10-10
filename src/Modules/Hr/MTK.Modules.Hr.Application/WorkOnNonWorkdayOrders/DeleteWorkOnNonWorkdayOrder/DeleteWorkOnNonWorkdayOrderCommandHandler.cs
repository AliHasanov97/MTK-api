using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.DeleteWorkOnNonWorkdayOrder;

internal sealed class DeleteWorkOnNonWorkdayOrderCommandHandler : ICommandHandler<DeleteWorkOnNonWorkdayOrderCommand>
{
    private readonly IWorkOnNonWorkdayOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteWorkOnNonWorkdayOrderCommandHandler(
        IWorkOnNonWorkdayOrderRepository repository,
        IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(DeleteWorkOnNonWorkdayOrderCommand request, CancellationToken cancellationToken)
    {
        var workOnNonWorkdayOrder = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (workOnNonWorkdayOrder is null)
            return Result.Failure(WorkOnNonWorkdayOrderErrors.NotFound);

        await _repository.DeleteAsync(workOnNonWorkdayOrder, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
