using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.CompensationOrders;
using MTK.Modules.Hr.Domain.CompensationOrders;

namespace MTK.Modules.Hr.Application.CompensationOrders.DeleteCompensationOrder;

internal sealed class DeleteCompensationOrderCommandHandler : ICommandHandler<DeleteCompensationOrderCommand>
{
    private readonly ICompensationOrderRepository _repository;

    public DeleteCompensationOrderCommandHandler(ICompensationOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(
        DeleteCompensationOrderCommand request,
        CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (order == null)
            return Result.Failure(CompensationOrderErrors.NotFound(request.Id));

        await _repository.DeleteAsync(request.Id, null, cancellationToken);

        return Result.Success();
    }
}
