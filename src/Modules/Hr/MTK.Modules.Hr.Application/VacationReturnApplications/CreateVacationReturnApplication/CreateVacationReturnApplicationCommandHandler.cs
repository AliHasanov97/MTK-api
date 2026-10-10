using MTK.Modules.Hr.Application.Services;
using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Application.Employees;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.VacationReturnApplications;

namespace MTK.Modules.Hr.Application.VacationReturnApplications.CreateVacationReturnApplication;

internal sealed class CreateVacationReturnApplicationCommandHandler
    : ICommandHandler<CreateVacationReturnApplicationCommand, ResponseObjectWithName>
{
    private readonly IVacationReturnApplicationRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaveOverlapService _leaveOverlap;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public CreateVacationReturnApplicationCommandHandler(
        IVacationReturnApplicationRepository repository,
        IEmployeeRepository employeeRepository,
        IUnitOfWork unitOfWork,
        ILeaveOverlapService leaveOverlap,
        IMapper mapper,
        IUserContext userContext)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _unitOfWork = unitOfWork;
        _leaveOverlap = leaveOverlap;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<Result<ResponseObjectWithName>> Handle(
        CreateVacationReturnApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Employee yoxla
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result.Failure<ResponseObjectWithName>(EmployeeErrors.NotFound);

        var valid = await _leaveOverlap.EnsureReturnDateIsValidAsync(
            request.EmployeeId, request.ReturnDate, cancellationToken);
        if (valid.IsFailure)
            return Result.Failure<ResponseObjectWithName>(valid.Error);

        // 2. Application yarat (CompanyId employee-dən götürülür)
        var applicationResult = VacationReturnApplication.Create(
            request.EmployeeId,
            request.ReturnDate,
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