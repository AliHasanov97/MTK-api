using MTK.Modules.Hr.Application.Services;
using AutoMapper;
using MTK.Common.Presentation.Responses;
using MTK.Common.Application.Messaging;
using MTK.Common.Domain.Abstractions;
using MTK.Modules.Hr.Application.Abstractions.Authentication;
using MTK.Modules.Hr.Application.Abstractions.Data;
using MTK.Modules.Hr.Domain.Applications;
using MTK.Modules.Hr.Domain.Employees;
using MTK.Modules.Hr.Domain.UnpaidLeaveApplications;

namespace MTK.Modules.Hr.Application.UnpaidLeaveApplications.CreateUnpaidLeaveApplication;

internal sealed class CreateUnpaidLeaveApplicationCommandHandler
    : ICommandHandler<CreateUnpaidLeaveApplicationCommand, ResponseObjectWithName>
{
    private readonly IUnpaidLeaveApplicationRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILeaveOverlapService _leaveOverlap;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;

    public CreateUnpaidLeaveApplicationCommandHandler(
        IUnpaidLeaveApplicationRepository repository,
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
        CreateUnpaidLeaveApplicationCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Employee yoxla
        var employee = await _employeeRepository.GetByIdDefaultAsync(request.EmployeeId, cancellationToken);
        if (employee == null)
            return Result.Failure<ResponseObjectWithName>(
                Application.Employees.EmployeeErrors.NotFound);

        var overlap = await _leaveOverlap.EnsureNoOverlapAsync(
            request.EmployeeId, request.StartDate, request.EndDate, null, cancellationToken);
        if (overlap.IsFailure)
            return Result.Failure<ResponseObjectWithName>(overlap.Error);

        // 2. Application yarat (CompanyId employee-dən götürülür)
        var applicationResult = UnpaidLeaveApplication.Create(
            request.EmployeeId,
            request.StartDate,
            request.EndDate,
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
