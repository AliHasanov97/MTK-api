using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;
using MTK.Modules.Hr.Domain.UnpaidLeaveOrders;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.ConvertToOrder;

internal sealed class ConvertUnpaidLeaveToOrderCommandHandler
    : ICommandHandler<ConvertUnpaidLeaveToOrderCommand, ConvertUnpaidLeaveToOrderResponse>
{
    private readonly IUnpaidLeaveApplicationRepository _unpaidLeaveApplicationRepository;
    private readonly IUnpaidLeaveOrderRepository _unpaidLeaveOrderRepository;
    private readonly ReturnToWorkDateService _returnToWorkDateService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertUnpaidLeaveToOrderCommandHandler(
        IUnpaidLeaveApplicationRepository unpaidLeaveApplicationRepository,
        IUnpaidLeaveOrderRepository unpaidLeaveOrderRepository,
        ReturnToWorkDateService returnToWorkDateService,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _unpaidLeaveApplicationRepository = unpaidLeaveApplicationRepository;
        _unpaidLeaveOrderRepository = unpaidLeaveOrderRepository;
        _returnToWorkDateService = returnToWorkDateService;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertUnpaidLeaveToOrderResponse>> Handle(
        ConvertUnpaidLeaveToOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. UnpaidLeaveApplication götür
        var application = await _unpaidLeaveApplicationRepository.GetByIdDefaultAsync(
            request.UnpaidLeaveApplicationId,
            cancellationToken);

        if (application == null)
            return Result.Failure<ConvertUnpaidLeaveToOrderResponse>(
                UnpaidLeaveApplicationErrors.NotFound);

        // 2. Artıq order yaradılıbmı? (Status check)
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<ConvertUnpaidLeaveToOrderResponse>(
                UnpaidLeaveApplicationErrors.AlreadyConvertedToOrder);

        // 3. İşə qayıtma tarixini hesabla
        var endDateOnly = DateOnly.FromDateTime(application.EndDate.UtcDateTime);
        var returnToWorkResult = await _returnToWorkDateService.CalculateAsync(
            application.EmployeeId,
            endDateOnly,
            cancellationToken);

        if (returnToWorkResult.IsFailure)
            return Result.Failure<ConvertUnpaidLeaveToOrderResponse>(returnToWorkResult.Error);

        // 4. UnpaidLeaveOrder yarat
        var order = UnpaidLeaveOrder.Create(
            application.Id,
            application.EmployeeId,
            application.StartDate,
            application.EndDate,
            returnToWorkResult.Value,
            _userContext.UserId,
            application.Notes
        );

        // 5. Application statusunu yenilə
        application.MarkAsConvertedToOrder();

        // 6. Saxla
        await _unpaidLeaveOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Response qaytarır
        return Result.Success(new ConvertUnpaidLeaveToOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        });
    }
}
