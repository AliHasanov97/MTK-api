using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.EducationLeaveApplications;
using MTK.Modules.Hr.Domain.EducationLeaveOrders;

namespace MTK.Modules.Hr.Application.EducationLeaveApplications.ConvertToOrder;

internal sealed class ConvertEducationLeaveToOrderCommandHandler
    : ICommandHandler<ConvertEducationLeaveToOrderCommand, ConvertEducationLeaveToOrderResponse>
{
    private readonly IEducationLeaveApplicationRepository _educationLeaveApplicationRepository;
    private readonly IEducationLeaveOrderRepository _educationLeaveOrderRepository;
    private readonly ReturnToWorkDateService _returnToWorkDateService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertEducationLeaveToOrderCommandHandler(
        IEducationLeaveApplicationRepository educationLeaveApplicationRepository,
        IEducationLeaveOrderRepository educationLeaveOrderRepository,
        ReturnToWorkDateService returnToWorkDateService,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _educationLeaveApplicationRepository = educationLeaveApplicationRepository;
        _educationLeaveOrderRepository = educationLeaveOrderRepository;
        _returnToWorkDateService = returnToWorkDateService;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertEducationLeaveToOrderResponse>> Handle(
        ConvertEducationLeaveToOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. EducationLeaveApplication götür
        var application = await _educationLeaveApplicationRepository.GetByIdDefaultAsync(
            request.EducationLeaveApplicationId,
            cancellationToken);

        if (application == null)
            return Result.Failure<ConvertEducationLeaveToOrderResponse>(
                EducationLeaveApplicationErrors.NotFound);

        // 2. Artıq order yaradılıbmı? (Status check)
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<ConvertEducationLeaveToOrderResponse>(
                EducationLeaveApplicationErrors.AlreadyConvertedToOrder);

        // 3. İşə qayıtma tarixini hesabla
        var endDateOnly = DateOnly.FromDateTime(application.EndDate.UtcDateTime);
        var returnToWorkResult = await _returnToWorkDateService.CalculateAsync(
            application.EmployeeId,
            endDateOnly,
            cancellationToken);

        if (returnToWorkResult.IsFailure)
            return Result.Failure<ConvertEducationLeaveToOrderResponse>(returnToWorkResult.Error);

        // 4. EducationLeaveOrder yarat
        var order = EducationLeaveOrder.Create(
            application.Id,
            application.EmployeeId,
            application.StartDate,
            application.EndDate,
            returnToWorkResult.Value,
            _userContext.UserId,
            application.Reason
        );

        // 5. Application statusunu yenilə
        application.MarkAsConvertedToOrder();

        // 6. Saxla
        await _educationLeaveOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Response qaytarır
        return Result.Success(new ConvertEducationLeaveToOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        });
    }
}
