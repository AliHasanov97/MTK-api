using MTK.Common.Application.Messaging;

using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.OrdersForChangeOfPosition;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.ApplicationsForChangeOfPosition;
using MTK.Modules.Hr.Domain.Jobs;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.OrdersForChangeOfPosition;

namespace MTK.Modules.Hr.Application.ApplicationsForChangeOfPosition.ConvertToOrderForChangeOfPosition;

internal sealed class ConvertApplicationForChangeOfPositionToOrderCommandHandler
    : ICommandHandler<ConvertApplicationForChangeOfPositionToOrderCommand, ConvertApplicationForChangeOfPositionToOrderResponse>
{
    private readonly IApplicationForChangeOfPositionRepository _applicationRepository;
    private readonly IOrderForChangeOfPositionRepository _orderForChangeOfPositionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertApplicationForChangeOfPositionToOrderCommandHandler(
        IApplicationForChangeOfPositionRepository applicationRepository,
        IOrderForChangeOfPositionRepository orderForChangeOfPositionRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _applicationRepository = applicationRepository;
        _orderForChangeOfPositionRepository = orderForChangeOfPositionRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertApplicationForChangeOfPositionToOrderResponse>> Handle(
        ConvertApplicationForChangeOfPositionToOrderCommand request,
        CancellationToken cancellationToken)
    {
        try
        {
            var application = await _applicationRepository.GetByIdDefaultAsync(request.ApplicationId, cancellationToken);
            if (application is null)
                return Result.Failure<ConvertApplicationForChangeOfPositionToOrderResponse>(
                    ApplicationForChangeOfPositionErrors.NotFound);

            if (application.Status == ApplicationStatus.ConvertedToOrder)
                return Result.Failure<ConvertApplicationForChangeOfPositionToOrderResponse>(
                    ApplicationForChangeOfPositionErrors.AlreadyConverted);

            var order = OrderForChangeOfPosition.Create(
                application.Id,
                application.EmployeeId,
                application.NewJobId,
                _userContext.UserId);

            application.MarkAsConvertedToOrder();

            _orderForChangeOfPositionRepository.Add(order);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success(new ConvertApplicationForChangeOfPositionToOrderResponse
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber
            });
        }
        catch (NullReferenceException)
        {
            return Result.Failure<ConvertApplicationForChangeOfPositionToOrderResponse>(
                ApplicationForChangeOfPositionErrors.NotFound);
        }
    }
}
