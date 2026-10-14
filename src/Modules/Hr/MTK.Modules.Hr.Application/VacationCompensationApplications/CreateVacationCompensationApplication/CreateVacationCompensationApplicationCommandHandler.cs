using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.VacationBalances.Services;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationCompensationApplications;

namespace MTK.Modules.Hr.Application.VacationCompensationApplications.CreateVacationCompensationApplication;

internal sealed class CreateVacationCompensationApplicationCommandHandler
    : ICommandHandler<CreateVacationCompensationApplicationCommand, ResponseObjectWithName>
{
    private readonly IVacationCompensationApplicationRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IVacationBalanceService _vacationBalanceService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public CreateVacationCompensationApplicationCommandHandler(
        IVacationCompensationApplicationRepository repository,
        IEmployeeRepository employeeRepository,
        IVacationBalanceService vacationBalanceService,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
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

        // 1.5. Qalan məzuniyyət balansını yoxla (WorkYearStart verilibsə həmin iş ili, yoxsa cari)
        var asOfDate = request.WorkYearStart.HasValue
            ? new DateTimeOffset(request.WorkYearStart.Value.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero)
            : (DateTimeOffset?)null;

        var balanceResult = await _vacationBalanceService.EnsureCurrentAsync(
            request.EmployeeId, asOfDate, cancellationToken);
        if (balanceResult.IsFailure)
            return Result.Failure<ResponseObjectWithName>(balanceResult.Error);

        if (request.RequestedDays > balanceResult.Value.RemainingDays)
            return Result.Failure<ResponseObjectWithName>(
                VacationCompensationApplicationErrors.ExceedsBalance(request.RequestedDays, balanceResult.Value.RemainingDays));

        // 2. Application yarat
        var applicationResult = VacationCompensationApplication.Create(
            request.EmployeeId,
            request.RequestedDays,
            _userContext.UserId,
            request.Notes,
            request.WorkYearStart,
            request.WorkYearEnd
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
