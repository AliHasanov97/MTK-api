using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeApplications;
using MTK.Modules.Hr.Domain.EmploymentStatusChangeOrders;
using MTK.Modules.Hr.Domain.Orders;

namespace MTK.Modules.Hr.Application.EmploymentStatusChangeApplications.ConvertToOrder;

internal sealed class ConvertEmploymentStatusChangeApplicationToOrderCommandHandler
    : ICommandHandler<ConvertEmploymentStatusChangeApplicationToOrderCommand, ConvertEmploymentStatusChangeApplicationToOrderResponse>
{
    private readonly IEmploymentStatusChangeApplicationRepository _applicationRepository;
    private readonly IEmploymentStatusChangeOrderRepository _orderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertEmploymentStatusChangeApplicationToOrderCommandHandler(
        IEmploymentStatusChangeApplicationRepository applicationRepository,
        IEmploymentStatusChangeOrderRepository orderRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _applicationRepository = applicationRepository;
        _orderRepository = orderRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertEmploymentStatusChangeApplicationToOrderResponse>> Handle(
        ConvertEmploymentStatusChangeApplicationToOrderCommand request,
        CancellationToken cancellationToken)
    {
        var application = await _applicationRepository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);
        if (application is null)
            return Result.Failure<ConvertEmploymentStatusChangeApplicationToOrderResponse>(
                EmploymentStatusChangeApplicationErrors.NotFound);

        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<ConvertEmploymentStatusChangeApplicationToOrderResponse>(
                EmploymentStatusChangeApplicationErrors.AlreadyConverted);

        var order = EmploymentStatusChangeOrder.Create(
            application.Id,
            application.EmployeeId,
            application.CurrentEmploymentType,
            application.NewEmploymentType,
            application.OrderExecutionSupervisorId,
            _userContext.UserId);

        application.MarkAsConvertedToOrder();

        _orderRepository.Add(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(new ConvertEmploymentStatusChangeApplicationToOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        });
    }
}
