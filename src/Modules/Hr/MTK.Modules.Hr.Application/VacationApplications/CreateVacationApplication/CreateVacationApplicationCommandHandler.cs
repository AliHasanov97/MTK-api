using MTK.Modules.Hr.Application.Services;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Application.VacationApplications.Services;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationApplications;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationApplications.CreateVacationApplication;

internal sealed class CreateVacationApplicationCommandHandler
    : ICommandHandler<CreateVacationApplicationCommand, CreateVacationApplicationResponse>
{
    private readonly IVacationApplicationRepository _vacationApplicationRepository;
    private readonly IVacationApplicationAllocationService _allocationService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaveOverlapService _leaveOverlap;
    private readonly IVacationBalanceService _vacationBalanceService;
    private readonly IVacationBalanceRepository _vacationBalanceRepository;
    private readonly IUserContext _userContext;

    public CreateVacationApplicationCommandHandler(
        IVacationApplicationRepository vacationApplicationRepository,
        IVacationApplicationAllocationService allocationService,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ILeaveOverlapService leaveOverlap,
        IVacationBalanceService vacationBalanceService,
        IVacationBalanceRepository vacationBalanceRepository,
        IUserContext userContext)
    {
        _vacationApplicationRepository = vacationApplicationRepository;
        _allocationService = allocationService;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _leaveOverlap = leaveOverlap;
        _vacationBalanceService = vacationBalanceService;
        _vacationBalanceRepository = vacationBalanceRepository;
        _userContext = userContext;
    }

    public async Task<Result<CreateVacationApplicationResponse>> Handle(
        CreateVacationApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check employee exists
        var employee = await _employeeRepository.GetByIdDefaultAsync(
            request.EmployeeId, cancellationToken);
        if (employee is null)
            return Result.Failure<CreateVacationApplicationResponse>(
                EmployeeErrors.NotFound);

        // 2. Calculate RequestedDays and EndDate (bayram və həftə sonları nəzərə alınır)
        var calculationResult = await _allocationService.CalculateDaysAndEndDateAsync(
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
            request.RequestedDays,
            cancellationToken);

        if (calculationResult.IsFailure)
            return Result.Failure<CreateVacationApplicationResponse>(
                calculationResult.Error);

        var (requestedDays, endDate) = calculationResult.Value;

        var overlap = await _leaveOverlap.EnsureNoOverlapAsync(
            request.EmployeeId, request.StartDate, endDate, null, cancellationToken);
        if (overlap.IsFailure)
            return Result.Failure<CreateVacationApplicationResponse>(overlap.Error);

        // 2.5. Balansı müəyyənləşdir və qalan günləri yoxla.
        // İstifadəçi konkret balans seçibsə, onu istifadə edirik — amma mütləq həmin işçinin
        // balansı olmalıdır. Seçməyibsə (boş GUID), StartDate-in iş ilinə uyğun balans götürülür.
        // Əvvəl boş GUID birbaşa saxlanılırdı və
        // FK_vacation_applications_vacation_balances_VacationBalanceId pozulurdu.
        VacationBalance balance;
        if (request.VacationBalanceId == Guid.Empty)
        {
            var balanceResult = await _vacationBalanceService.EnsureCurrentAsync(
                request.EmployeeId, request.StartDate, cancellationToken);
            if (balanceResult.IsFailure)
                return Result.Failure<CreateVacationApplicationResponse>(balanceResult.Error);

            balance = balanceResult.Value;
        }
        else
        {
            var selectedBalance = await _vacationBalanceRepository.GetByIdAsync(
                request.VacationBalanceId, cancellationToken);

            if (selectedBalance is null)
                return Result.Failure<CreateVacationApplicationResponse>(
                    VacationApplicationErrors.BalanceNotFound);

            if (selectedBalance.EmployeeId != request.EmployeeId)
                return Result.Failure<CreateVacationApplicationResponse>(
                    VacationApplicationErrors.BalanceEmployeeMismatch);

            balance = selectedBalance;
        }

        if (requestedDays > balance.RemainingDays)
            return Result.Failure<CreateVacationApplicationResponse>(
                VacationApplicationErrors.ExceedsBalance(requestedDays, balance.RemainingDays));

        // 3. Create VacationApplication
        var applicationResult = VacationApplication.Create(
            request.EmployeeId,
            balance.Id,
            request.StartDate,
            endDate,
            requestedDays,
            _userContext.UserId,
            request.Notes);

        if (applicationResult.IsFailure)
            return Result.Failure<CreateVacationApplicationResponse>(
                applicationResult.Error);

        var application = applicationResult.Value;

        // 4. Save
        _vacationApplicationRepository.Add(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateVacationApplicationResponse(application.Id);
    }
}
