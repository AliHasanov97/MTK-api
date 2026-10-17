using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Services;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Orders;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationBalances;
using MTK.Modules.Hr.Domain.VacationOrders;

namespace MTK.Modules.Hr.Application.VacationApplications.ConvertToOrder;

internal sealed class ConvertVacationToOrderCommandHandler
    : ICommandHandler<ConvertVacationToOrderCommand, ConvertVacationToOrderResponse>
{
    private readonly IVacationApplicationRepository _vacationApplicationRepository;
    private readonly IVacationOrderRepository _vacationOrderRepository;
    private readonly ReturnToWorkDateService _returnToWorkDateService;
    private readonly IVacationBalanceService _vacationBalanceService;
    private readonly IVacationBalanceRepository _vacationBalanceRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertVacationToOrderCommandHandler(
        IVacationApplicationRepository vacationApplicationRepository,
        IVacationOrderRepository vacationOrderRepository,
        ReturnToWorkDateService returnToWorkDateService,
        IVacationBalanceService vacationBalanceService,
        IVacationBalanceRepository vacationBalanceRepository,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _vacationApplicationRepository = vacationApplicationRepository;
        _vacationOrderRepository = vacationOrderRepository;
        _returnToWorkDateService = returnToWorkDateService;
        _vacationBalanceService = vacationBalanceService;
        _vacationBalanceRepository = vacationBalanceRepository;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertVacationToOrderResponse>> Handle(
        ConvertVacationToOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. VacationApplication götür 
        var application = await _vacationApplicationRepository.GetByIdWithDetailsAsync(
            request.VacationApplicationId,
            cancellationToken);

        if (application == null)
            return Result.Failure<ConvertVacationToOrderResponse>(
                VacationApplicationErrors.NotFound);

        // 2. Artıq order yaradılıbmı? (Status check)
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<ConvertVacationToOrderResponse>(
                VacationApplicationErrors.AlreadyConvertedToOrder);

        // 3. İşə qayıtma tarixini hesabla
        var endDateOnly = DateOnly.FromDateTime(application.EndDate!.Value.UtcDateTime);
        var returnToWorkResult = await _returnToWorkDateService.CalculateAsync(
            application.EmployeeId,
            endDateOnly,
            cancellationToken);

        if (returnToWorkResult.IsFailure)
            return Result.Failure<ConvertVacationToOrderResponse>(returnToWorkResult.Error);

        // 4. VacationOrder yarat
        var order = VacationOrder.Create(
            application.Id,
            application.EmployeeId,
            application.VacationBalanceId,
            application.StartDate,
            application.EndDate!.Value,
            application.TotalRequestedDays,
            returnToWorkResult.Value,
            _userContext.UserId,
            application.Notes
        );

        // 5. Application statusunu yenilə
        application.MarkAsConvertedToOrder();

        // 5.5. FIFO qaydası ilə bütün balanslardan günləri çıx
        // İşçinin bütün balanslarını ən köhnədən başlayaraq götür
        var allBalances = await _vacationBalanceRepository.GetAllByEmployeeIdAsync(
            application.EmployeeId, cancellationToken);

        // WorkYearStart-a görə sırala (ən köhnə birinci)
        var sortedBalances = allBalances.OrderBy(b => b.WorkYearStart).ToList();

        var remainingDays = application.TotalRequestedDays;
        foreach (var balance in sortedBalances)
        {
            if (remainingDays <= 0) break;

            var daysToTake = Math.Min(remainingDays, balance.RemainingDays);
            if (daysToTake > 0)
            {
                balance.RegisterUsedDays(daysToTake);
                remainingDays -= daysToTake;
            }
        }

        // 6. Saxla
        await _vacationOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 7. Response qaytarır
        return Result.Success(new ConvertVacationToOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        });
    }
}
