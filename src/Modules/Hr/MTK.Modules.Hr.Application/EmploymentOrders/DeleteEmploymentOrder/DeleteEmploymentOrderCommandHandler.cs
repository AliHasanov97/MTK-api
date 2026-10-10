using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.EmploymentOrders;
using MediatR;

namespace MTK.Modules.Hr.Application.EmploymentOrders.DeleteEmploymentOrder;

internal sealed class DeleteEmploymentOrderCommandHandler : ICommandHandler<DeleteEmploymentOrderCommand, Unit>
{
    private readonly IEmploymentOrderRepository _employmentOrderRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEmploymentOrderCommandHandler(
        IEmploymentOrderRepository employmentOrderRepository,
        IUnitOfWork unitOfWork)
    {
        _employmentOrderRepository = employmentOrderRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Unit>> Handle(DeleteEmploymentOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _employmentOrderRepository.GetByIdDefaultAsync(request.Id, cancellationToken);
        if (order is null)
            return Result.Failure<Unit>(EmploymentOrderErrors.NotFound);

        await _employmentOrderRepository.DeleteAsync(order, null, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(Unit.Value);
    }
}
