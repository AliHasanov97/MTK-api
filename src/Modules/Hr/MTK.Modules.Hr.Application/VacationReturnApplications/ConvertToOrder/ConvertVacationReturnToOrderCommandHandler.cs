using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.ConvertToOrder;

internal sealed class ConvertVacationReturnToOrderCommandHandler
    : ICommandHandler<ConvertVacationReturnToOrderCommand, ConvertVacationReturnToOrderResponse>
{
    private readonly IVacationReturnApplicationRepository _vacationReturnApplicationRepository;
    private readonly IVacationReturnOrderRepository _vacationReturnOrderRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertVacationReturnToOrderCommandHandler(
        IVacationReturnApplicationRepository vacationReturnApplicationRepository,
        IVacationReturnOrderRepository vacationReturnOrderRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _vacationReturnApplicationRepository = vacationReturnApplicationRepository;
        _vacationReturnOrderRepository = vacationReturnOrderRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertVacationReturnToOrderResponse>> Handle(
        ConvertVacationReturnToOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. VacationReturnApplication götür
        var application = await _vacationReturnApplicationRepository.GetByIdDefaultAsync(
            request.VacationReturnApplicationId,
            cancellationToken);

        if (application == null)
            return Result.Failure<ConvertVacationReturnToOrderResponse>(
                VacationReturnApplicationErrors.NotFound);

        // 2. Artıq order yaradılıbmı? (Status check)
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<ConvertVacationReturnToOrderResponse>(
                VacationReturnApplicationErrors.AlreadyConvertedToOrder);

        // 3. VacationReturnOrder yarat
        var order = VacationReturnOrder.Create(
            application.Id,
            application.EmployeeId,
            application.ReturnDate,
            _userContext.UserId,
            application.Notes
        );

        // 4. Application statusunu yenilə
        application.MarkAsConvertedToOrder();

        // 5. Saxla
        await _vacationReturnOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Response qaytarır
        return Result.Success(new ConvertVacationReturnToOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        });
    }
}