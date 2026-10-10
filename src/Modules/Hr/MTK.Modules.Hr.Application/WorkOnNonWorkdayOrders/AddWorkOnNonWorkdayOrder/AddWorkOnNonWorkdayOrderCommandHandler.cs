using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.WorkOnNonWorkdayOrders;

namespace MTK.Modules.Hr.Application.WorkOnNonWorkdayOrders.AddWorkOnNonWorkdayOrder;

internal sealed class AddWorkOnNonWorkdayOrderCommandHandler : ICommandHandler<AddWorkOnNonWorkdayOrderCommand, AddWorkOnNonWorkdayOrderResponse>
{
    private readonly IWorkOnNonWorkdayOrderRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public AddWorkOnNonWorkdayOrderCommandHandler(
        IWorkOnNonWorkdayOrderRepository repository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<AddWorkOnNonWorkdayOrderResponse>> Handle(AddWorkOnNonWorkdayOrderCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var workOnNonWorkdayOrder = WorkOnNonWorkdayOrder.Create(
                request.StartDate,
                request.EndDate,
                _userContext.UserId);

            await _repository.AddAsync(workOnNonWorkdayOrder, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new AddWorkOnNonWorkdayOrderResponse
            {
                Id = workOnNonWorkdayOrder.Id,
                OrderNumber = workOnNonWorkdayOrder.OrderNumber
            });
        }
        catch (Exception)
        {
            return Result.Failure<AddWorkOnNonWorkdayOrderResponse>(WorkOnNonWorkdayOrderErrors.AddFailed);
        }
    }
}
