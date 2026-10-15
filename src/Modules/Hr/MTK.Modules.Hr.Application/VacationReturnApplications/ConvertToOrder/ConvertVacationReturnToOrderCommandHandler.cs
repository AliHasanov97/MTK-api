using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.CalendarDays;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.Services;
using MTK.Modules.Hr.Domain.VacationOrders;
using MTK.Modules.Hr.Domain.VacationReturnApplications;
using MTK.Modules.Hr.Domain.VacationReturnOrders;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.ConvertToOrder;

internal sealed class ConvertVacationReturnToOrderCommandHandler
    : ICommandHandler<ConvertVacationReturnToOrderCommand, ConvertVacationReturnToOrderResponse>
{
    private readonly IVacationReturnApplicationRepository _vacationReturnApplicationRepository;
    private readonly IVacationReturnOrderRepository _vacationReturnOrderRepository;
    private readonly IVacationOrderRepository _vacationOrderRepository;
    private readonly ICalendarDayRepository _calendarDayRepository;
    private readonly WorkingDayCalculatorService _workingDayCalculator;
    private readonly IVacationBalanceService _vacationBalanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserContext _userContext;

    public ConvertVacationReturnToOrderCommandHandler(
        IVacationReturnApplicationRepository vacationReturnApplicationRepository,
        IVacationReturnOrderRepository vacationReturnOrderRepository,
        IVacationOrderRepository vacationOrderRepository,
        ICalendarDayRepository calendarDayRepository,
        WorkingDayCalculatorService workingDayCalculator,
        IVacationBalanceService vacationBalanceService,
        IUnitOfWork unitOfWork,
        IUserContext userContext)
    {
        _vacationReturnApplicationRepository = vacationReturnApplicationRepository;
        _vacationReturnOrderRepository = vacationReturnOrderRepository;
        _vacationOrderRepository = vacationOrderRepository;
        _calendarDayRepository = calendarDayRepository;
        _workingDayCalculator = workingDayCalculator;
        _vacationBalanceService = vacationBalanceService;
        _unitOfWork = unitOfWork;
        _userContext = userContext;
    }

    public async Task<Result<ConvertVacationReturnToOrderResponse>> Handle(
        ConvertVacationReturnToOrderCommand request,
        CancellationToken cancellationToken)
    {
        // 1. VacationReturnApplication götür (Employee daxil — WorkingDays lazımdır)
        var application = await _vacationReturnApplicationRepository.GetByIdWithDetailsAsync(
            request.VacationReturnApplicationId,
            cancellationToken);

        if (application == null)
            return Result.Failure<ConvertVacationReturnToOrderResponse>(
                VacationReturnApplicationErrors.NotFound);

        // 2. Artıq order yaradılıbmı? (Status check)
        if (application.Status == ApplicationStatus.ConvertedToOrder)
            return Result.Failure<ConvertVacationReturnToOrderResponse>(
                VacationReturnApplicationErrors.AlreadyConvertedToOrder);

        var returnDateOnly = DateOnly.FromDateTime(application.ReturnDate.UtcDateTime);

        // 3. Geri çağırılan ƏMƏK MƏZUNIYYƏTI əmrini tap (balansdan düzgün günü çıxmaq üçün lazımdır —
        // bax: LeaveOverlapService.EnsureReturnDateIsValidAsync, eyni sorğu ərizə yaradılanda da işlədilib).
        var originalVacationOrder = (await _vacationOrderRepository.ListAsync(
                o => o.EmployeeId == application.EmployeeId
                     && o.StartDate <= application.ReturnDate
                     && o.EndDate >= application.ReturnDate,
                cancellationToken))
            .FirstOrDefault();

        if (originalVacationOrder is null)
            return Result.Failure<ConvertVacationReturnToOrderResponse>(
                VacationReturnApplicationErrors.OriginalVacationOrderNotFound);

        // 4. İşçi məzuniyyəti vaxtından əvvəl kəsdiyi üçün istifadə olunmamış gün sayını hesabla
        // (eyni sayma metodu ilə ki, günlər əslində çıxılarkən istifadə olunub — bax: WorkingDayCalculatorService.CountVacationDays).
        var originalEndDateOnly = DateOnly.FromDateTime(originalVacationOrder.EndDate.UtcDateTime);
        var calendarDays = await _calendarDayRepository.GetByDateRangeAsync(
            returnDateOnly, originalEndDateOnly, cancellationToken);
        var unusedDays = _workingDayCalculator.CountVacationDays(
            returnDateOnly, originalEndDateOnly, application.Employee.WorkingDays ?? WorkingDays.FiveDays, calendarDays);

        // 5. VacationReturnOrder yarat
        var order = VacationReturnOrder.Create(
            application.Id,
            application.EmployeeId,
            application.ReturnDate,
            _userContext.UserId,
            unusedDays,
            application.Notes
        );

        // 6. Application statusunu yenilə
        application.MarkAsConvertedToOrder();

        // 6.5. Həmin günləri balansa qaytar (iş ili əsl məzuniyyətin başlama tarixinə görə —
        // RegisterUsedDays-in tərsi, bax: ConvertVacationToOrderCommandHandler).
        var balanceResult = await _vacationBalanceService.EnsureCurrentAsync(
            application.EmployeeId, originalVacationOrder.StartDate, cancellationToken);
        if (balanceResult.IsFailure)
            return Result.Failure<ConvertVacationReturnToOrderResponse>(balanceResult.Error);

        balanceResult.Value.ReturnUnusedDays(unusedDays);

        // 7. Saxla
        await _vacationReturnOrderRepository.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 8. Response qaytarır
        return Result.Success(new ConvertVacationReturnToOrderResponse
        {
            OrderId = order.Id,
            OrderNumber = order.OrderNumber
        });
    }
}
