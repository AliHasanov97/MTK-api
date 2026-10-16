using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;
using MTK.Modules.Hr.Domain.VacationBalances;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;

internal sealed class CreateVacationCompensationApplicationCommandHandler
    : ICommandHandler<CreateVacationCompensationApplicationCommand, ResponseObjectWithName>
{
    private readonly IVacationCompensationApplicationRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationBalanceRepository _vacationBalanceRepository;
    private readonly IVacationBalanceService _vacationBalanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public CreateVacationCompensationApplicationCommandHandler(
        IVacationCompensationApplicationRepository repository,
        IEmployeeRepository employeeRepository,
        IVacationBalanceRepository vacationBalanceRepository,
        IVacationBalanceService vacationBalanceService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _vacationBalanceRepository = vacationBalanceRepository;
        _vacationBalanceService = vacationBalanceService;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<ResponseObjectWithName>> Handle(
        CreateVacationCompensationApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Employee yoxla
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result.Failure<ResponseObjectWithName>(
                Application.Employees.EmployeeErrors.NotFound);

        // 1.5. Balansı müəyyənləşdir və qalan günləri yoxla.
        // İstifadəçi konkret balans seçibsə, onu istifadə edirik — amma mütləq həmin işçinin
        // balansı olmalıdır. Seçməyibsə (boş GUID), cari tarixə uyğun balans götürülür.
        VacationBalance balance;
        if (request.VacationBalanceId == Guid.Empty)
        {
            // Boş GUID göndərilibsə - cari iş ilinin balansını avtomatik seç
            var balanceResult = await _vacationBalanceService.EnsureCurrentAsync(
                request.EmployeeId, DateTimeOffset.UtcNow, cancellationToken);
            if (balanceResult.IsFailure)
                return Result.Failure<ResponseObjectWithName>(balanceResult.Error);

            balance = balanceResult.Value;
        }
        else
        {
            // Konkret balans ID göndərilibsə - onu yoxla
            var selectedBalance = await _vacationBalanceRepository.GetByIdAsync(
                request.VacationBalanceId, cancellationToken);

            if (selectedBalance is null)
                return Result.Failure<ResponseObjectWithName>(
                    new Error("VacationBalance.NotFound", "Məzuniyyət balansı tapılmadı"));

            if (selectedBalance.EmployeeId != request.EmployeeId)
                return Result.Failure<ResponseObjectWithName>(
                    new Error("VacationBalance.EmployeeMismatch", "Məzuniyyət balansı bu işçiyə aid deyil"));

            balance = selectedBalance;
        }

        if (request.RequestedDays > balance.RemainingDays)
            return Result.Failure<ResponseObjectWithName>(
                VacationCompensationApplicationErrors.ExceedsBalance(request.RequestedDays, balance.RemainingDays));

        // 2. Application yarat
        var applicationResult = VacationCompensationApplication.Create(
            request.EmployeeId,
            balance.Id,
            request.RequestedDays,
            _userContext.UserId,
            request.Notes
        );

        if (applicationResult.IsFailure)
            return Result.Failure<ResponseObjectWithName>(applicationResult.Error);

        // 3. Saxla
        await _repository.AddAsync(applicationResult.Value, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 4. Response - sadə Id və Name (ApplicationNumber) qaytarırıq
        return Result.Success(_mapper.Map<ResponseObjectWithName>(applicationResult.Value));
    }
}
